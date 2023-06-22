using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Claims;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Repositories;
using TaskHive.Application.Services.Security;
using TaskHive.Application.Services.Email;
using TaskHive.Core.Enums;
using TaskHive.Application.Contracts.Requests;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AccessNotifierHandler _accessNotifier;
        private readonly SecurityHelper _securityHelper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            AccountRepository accountRepository = new();
            _configuration = configuration;
            _logger = logger;
            _accessNotifier = new AccessNotifierHandler(_configuration, _logger);
            _securityHelper = new SecurityHelper(accountRepository);
        }

        /// <summary>
        /// Provides a token for a given account credentials
        /// </summary>
        /// <response code="200">Token</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="404">Email not registered</response>
        /// <response code="409">Account not verified</response>
        /// <response code="500">Internal error</response>
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            AccountRepository accountRepository = new();
            try
            {
                Account existingAccount;
                existingAccount = await accountRepository.GetAccountByEmailAsync(user.Email);
                if (existingAccount == null) return NotFound(new { message = "Account does not exist. Please sign up." });
                if (existingAccount.AccountState != State.Active)
                {
                    EmailService emailService = new();
                    BackgroundJob.Enqueue(() => emailService.SendVerificationEmail(existingAccount));
                    return Conflict(new { message = "Account not verified. Check your email." });
                }

                if (existingAccount.SignUpType == SignUpType.Default && !_securityHelper.VerifyPassword(user.Password, existingAccount.HashedPassword))
                {
                    return BadRequest(new { message = "Invalid password." });
                }

                string resetPasswordToken = _securityHelper.GenerateRandomToken();

                var geolocation = await _accessNotifier.AccessLocationDetailsHandler(HttpContext.Connection.RemoteIpAddress, existingAccount, resetPasswordToken);

                SaveAccessLocation(existingAccount.AccountId, geolocation);

                var token = _securityHelper.GenerateJwtToken(_configuration, existingAccount);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Provides authentication from external source, can determine whether is for login or register an user
        /// </summary>
        /// <param name="provider">External authentication provider (Google)</param>
        /// <param name="action">Determine whether is intending to register or login an user (register, login)</param>
        /// <param name="company">When registering a new user, defines its company name</param>
        /// <response code="200">(login) - Token <br/> (register) - Account registered</response>
        /// <response code="400">(login) - Invalid credentials</response>
        /// <response code="404">(login) - Email not registered</response>
        /// <response code="409">(login) - Account not verified <br/> (register) - Email already registered</response>
        /// <response code="500">Internal error</response>
        [HttpGet("auth/external/{provider}")]
        public IActionResult ExternalAuthentication([FromRoute] string provider, [FromQuery] [Required] string action, [FromQuery] string company)
        {
            var properties = new AuthenticationProperties { RedirectUri = $"/api/auth/{action}/external/{provider}" };
            if (string.IsNullOrEmpty(action) || (action != "register" && action != "login")) return BadRequest(new { message = "Invalid action" });

            if (action.Equals("register")) properties.Items["company"] = company;
            return Challenge(properties, provider);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("auth/login/external/{provider}")]
        public async Task<IActionResult> HandleExternalLogin()
        {
            var hasError = HttpContext.Request.Query.ContainsKey("error");

            if (hasError) return Forbid();

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim =>
            new Claim(claim.Issuer, claim.OriginalIssuer, claim.Type, claim.Value));

            var email = claims.Select(e => e.Issuer).Where(e => e.Contains('@')).FirstOrDefault();

            return await Login(new LoginRequest { Email = email });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("auth/register/external/{provider}")]
        public async Task<IActionResult> HandleExternalRegistration(string provider)
        {
            var hasError = HttpContext.Request.Query.ContainsKey("error");

            if (hasError) return Forbid();

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var companyName = result.Properties.Items["company"];

            IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim =>
            new Claim(claim.Issuer, claim.OriginalIssuer, claim.Type, claim.Value));

            var firstName = claims.ElementAt(2).Issuer;
            var lastName = claims.ElementAt(3).Issuer;
            var email = claims.Select(e => e.Issuer).Where(e => e.Contains('@')).FirstOrDefault();

            SignUpType signUpType = SignUpType.Google;
            Enum.TryParse(provider, out signUpType);

            RegisterRequest request = new()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                CompanyName = companyName,
                Culture = "en-US",
                TimeZone = "00:00",
                SignUpMode = signUpType,
                MobileNumber = string.Empty
            };

            return await Register(request);
        }

        /// <summary>
        /// Creates an account
        /// </summary>
        /// <response code="200">Account created</response>
        /// <response code="409">Email already registered</response>
        /// <response code="500">Internal error</response>
        [HttpPost("auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest user)
        {
            AccountRepository accountRepository = new();
            CompanyRepository companyRepository = new();
            try
            {
                var alreadyExists = await accountRepository.GetActiveAccountByEmailAsync(user.Email);

                if (alreadyExists != null)
                {
                    return Conflict(new { message = "Email already registered." });
                }

                var geolocationDetails = await _accessNotifier.GetGeolocationDetails(HttpContext.Connection.RemoteIpAddress);
                if (geolocationDetails == null || geolocationDetails.Status == Entities.Enums.GeolocationStatus.Fail) _logger.LogWarning("Could not get geolocation details.");

                Company newCompany = new()
                {
                    CompanyId = Guid.NewGuid(),
                    CompanyState = State.Active,
                    Name = user.CompanyName
                };

                var companyCreated = await companyRepository.AddCompany(newCompany);
                if (!companyCreated) return Problem();

                var defaultSignUp = user.SignUpMode == SignUpType.Default;
                string hashedPassword = string.Empty;

                if (defaultSignUp) hashedPassword = _securityHelper.HashPassword(user.Password);

                Account newAccount = new()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AccountState = defaultSignUp ? State.Inactive : State.Active,
                    Email = user.Email,
                    MobileNumber = user.MobileNumber,
                    HashedPassword = hashedPassword,
                    AccountId = Guid.NewGuid(),
                    CompanyId = newCompany.CompanyId,
                    RoleId = RoleType.Administrator,
                    VerificationToken = _securityHelper.GenerateRandomToken(),
                    OriginCountry = geolocationDetails?.Country,
                    Culture = user.Culture ?? "en-US",
                    SignUpType = user.SignUpMode,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    TimeZone = user.TimeZone ?? "00:00",
                    VerifiedAt = defaultSignUp ? null : DateTime.UtcNow,
                };

                var accountCreated = await accountRepository.AddAccount(newAccount);
                if (!accountCreated) return Problem();

                EmailService emailService = new();
                BackgroundJob.Enqueue(() => emailService.SendVerificationEmail(newAccount));

                return Ok(new { message = "Account created." });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// List of account details for given emails
        /// </summary>
        /// <response code="200">List of found accounts</response>
        /// <response code="500">Internal error</response>
        [HttpPost("auth/search-accounts")]
        public async Task<IActionResult> GetAccountsByEmail(GetAccountsByEmailRequest request)
        {
            AccountRepository accountRepository = new();
            try
            {
                var accounts = await accountRepository.GetAccountDtos(request.EmailAddresses);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Account details for provided token
        /// </summary>
        /// <response code="200">Account details</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("auth/logged-account")]
        [Authorize]
        public async Task<IActionResult> GetLoggedAccount()
        {
            AccountRepository accountRepository = new();
            try
            {
                var email = User.FindFirst(ClaimTypes.Name)?.Value;
                var account = await accountRepository.GetAccountDto(email);
                if (account == null) return NotFound();

                return Ok(account);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Edits account details
        /// </summary>
        /// <response code="200">Account details</response>
        /// <response code="400">Token is not from given account</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Given email already exists</response>
        /// <response code="500">Internal error</response>
        [HttpPut("auth/account/edit")]
        [Authorize]
        public async Task<IActionResult> UpdateAccount(AccountDto account)
        {
            AccountRepository accountRepository = new();
            try
            {
                var email = User.FindFirst(ClaimTypes.Name)?.Value;
                var logged = await accountRepository.GetAccountDto(email);
                if (logged == null) return NotFound();

                if (logged.AccountId != account.AccountId) return BadRequest(new { message = "Invalid credentials" });

                if (account.Email != logged.Email)
                {
                    var exists = await accountRepository.GetAccountByEmailAsync(account.Email);
                    if (exists != null) return Conflict(new { message = "Email already exists!" });
                }

                var acc = await accountRepository.EditAccount(account);
                if (acc.AccountId != account.AccountId) return NotFound(new { message = "Please try again later!" });

                return Ok(acc);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Provides invite details
        /// </summary>
        /// <response code="200">Invite details</response>
        /// <response code="404">Invite not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("auth/invite/{inviteId}")]
        public async Task<IActionResult> GetInviteById(Guid inviteId)
        {
            InviteRepository inviteRepository = new();
            try
            {
                var invite = await inviteRepository.GetInviteDtoById(inviteId);
                if (invite == null) return NotFound();

                JsonSerializerSettings settings = new()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(invite, settings);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Creates an account using invite
        /// </summary>
        /// <response code="200">Account created</response>
        /// <response code="400">Invite expired</response>
        /// <response code="404">Invite not found</response>
        /// <response code="409">Given email already exists</response>
        /// <response code="500">Internal error</response>
        [HttpPost("auth/register/{inviteId}")]
        public async Task<IActionResult> RegisterFromInvite([FromBody] RegisterFromInviteRequest user, Guid inviteId)
        {
            InviteRepository inviteRepository = new();
            AccountRepository accountRepository = new();
            WorkspaceRepository workspaceRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();

            try
            {
                var invite = await inviteRepository.GetInviteByIdAsync(inviteId);

                if (invite == null) return NotFound(new { message = "Invalid invitation token." });

                if (!invite.IsActive || invite.ExpirationDate < DateTime.Now)
                {
                    return BadRequest(new { message = "Expired invitation token." });
                }

                var alreadyExists = await accountRepository.GetActiveAccountByEmailAsync(invite.InvitedEmail);

                Workspace workspace = await workspaceRepository.GetWorkspaceById(invite.WorkspaceId);
                if (alreadyExists != null)
                {
                    return Conflict(new { message = "Email already registered." });
                }

                var hashedPassword = _securityHelper.HashPassword(user.Password);

                Account newAccount = new()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AccountState = State.Active,
                    Email = invite.InvitedEmail,
                    MobileNumber = user.MobileNumber,
                    HashedPassword = hashedPassword,
                    AccountId = Guid.NewGuid(),
                    CompanyId = workspace.CompanyId,
                    RoleId = invite.RoleId,
                    SignUpType = SignUpType.Default,
                    Culture = user.Culture ?? "en-US",
                    TimeZone = user.TimeZone ?? "00:00",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var accountCreated = await accountRepository.AddAccount(newAccount);
                if (!accountCreated) return Problem();

                AccountWorkspace newAccWorkspace = new()
                {
                    AccountId = newAccount.AccountId,
                    AccountWorkspaceId = Guid.NewGuid(),
                    WorkspaceId = workspace.WorkspaceId,
                    RoleId = invite.RoleId
                };

                var accWorkspaceCreated = await accountWorkspaceRepository.AddAccountWorkspace(newAccWorkspace);
                if (!accWorkspaceCreated) return Problem();

                invite.IsActive = false;


                var inviteUpdated = await inviteRepository.UpdateInvite(invite);
                if (!inviteUpdated) return Problem();

                return Ok(new { message = "User created." });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Verifies an account using token
        /// </summary>
        /// <response code="200">Account verified</response>
        /// <response code="404">Verification token not found</response>
        /// <response code="409">Account already verified</response>
        /// <response code="500">Internal error</response>
        [HttpPost("auth/verify/{token}")]
        public async Task<IActionResult> Verify(string token)
        {
            AccountRepository accountRepository = new();
            try
            {
                Account existingAccount;
                existingAccount = await accountRepository.GetAccountByVerificationToken(token);
                if (existingAccount == null) return NotFound(new { message = "Invalid verification token." });
                if (existingAccount.AccountState == State.Active) return Conflict("Account already verified.");

                existingAccount.VerifiedAt = DateTime.UtcNow;
                existingAccount.AccountState = State.Active;

                var updatedAccount = await accountRepository.UpdateAccount(existingAccount);
                if (!updatedAccount) return Problem();

                return Ok(new { message = "Account verified." });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Sends as email for password reset
        /// </summary>
        /// <response code="200">Email will be sent</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Internal error</response>
        [HttpPost("auth/forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            AccountRepository accountRepository = new();
            try
            {
                Account existingAccount;
                existingAccount = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (existingAccount == null) return NotFound(new { message = "Account not found." });

                existingAccount.PasswordResetToken = _securityHelper.GenerateRandomToken();
                existingAccount.ResetTokenExpiration = DateTime.UtcNow.AddMinutes(15);

                var accountUpdated = await accountRepository.UpdateAccount(existingAccount);
                if (!accountUpdated) return Problem();

                var updatedAccount = await accountRepository.GetActiveAccountByEmailAsync(email);

                EmailService emailService = new();
                BackgroundJob.Enqueue(() => emailService.SendResetPasswordEmail(updatedAccount));

                return Ok(new { message = "Email will be sent." });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Updates a password 
        /// </summary>
        /// <response code="200">Password updated</response>
        /// <response code="400">Invalid reset password token</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Internal error</response>
        [HttpPut("auth/reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPassword)
        {
            AccountRepository accountRepository = new();
            try
            {
                Account existingAccount;
                existingAccount = await accountRepository.GetAccountByResetPasswordToken(resetPassword.Token);
                if (existingAccount == null) return NotFound(new { message = "Account not found." });
                if (existingAccount.ResetTokenExpiration < DateTime.UtcNow) return BadRequest(new { message = "Invalid reset password token." });

                var hashedPassword = _securityHelper.HashPassword(resetPassword.Password);

                existingAccount.HashedPassword = hashedPassword;
                existingAccount.PasswordResetToken = null;
                existingAccount.ResetTokenExpiration = null;

                var updatedAccount = await accountRepository.UpdateAccount(existingAccount);
                if (!updatedAccount) return Problem();

                return Ok(new { message = "Password updated." });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        private void SaveAccessLocation(Guid accountId, GeolocationDetails geolocationDetails)
        {
            AccountAccessLocationRepository accountAccessLocationRepository = new();
            AccountAccessLocation accessLocation = new()
            {
                AccountId = accountId,
                AccountAccessLocationId = Guid.NewGuid(),
                AccessDate = DateTime.UtcNow,
                IpAddress = geolocationDetails?.Query,
                City = geolocationDetails?.City,
                Country = geolocationDetails?.Country
            };

            accountAccessLocationRepository.AddAccessLocation(accessLocation);
        }

    }
}
