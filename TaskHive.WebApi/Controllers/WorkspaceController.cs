using Amazon.Auth.AccessControlPolicy;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Claims;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Repositories;
using TaskHive.Application.Services.Email;
using TaskHive.Core.Enums;
using TaskHive.Application.Contracts.Requests;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        /// <summary>
        /// Creates a workspace
        /// </summary>
        /// <response code="200">Workspace identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Not possible to create workspace</response>
        /// <response code="500">Internal error</response>
        [HttpPost("workspace")]
        [Authorize]
        public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest workspace)
        {
            AccountRepository accountRepository = new();
            WorkspaceRepository workspaceRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            WorkspaceValuePerHourRepository workspaceValuePerHourRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                Workspace newWorkspace = new()
                {
                    Name = workspace.WorkspaceName,
                    WorkspaceId = Guid.NewGuid(),
                    CompanyId = user.CompanyId,
                    HasHourlyRate = workspace.HasValuePerHour,
                    Abbreviation = workspace.Abbreviation
                };

                if (newWorkspace.HasHourlyRate)
                    newWorkspace.Currency = workspace.Currency;

                var workspaceCreated = await workspaceRepository.AddWorkspace(newWorkspace);
                if (!workspaceCreated) return Conflict(new { message = "Not possible to add workspace." });

                AccountWorkspace newAccWorkspace = new()
                {
                    AccountId = user.AccountId,
                    AccountWorkspaceId = Guid.NewGuid(),
                    WorkspaceId = newWorkspace.WorkspaceId,
                    RoleId = user.RoleId
                };

                var accWorkspaceCreated = await accountWorkspaceRepository.AddAccountWorkspace(newAccWorkspace);
                if (!accWorkspaceCreated) return Problem();

                if (newWorkspace.HasHourlyRate)
                {
                    var existingIssueTypeIds = workspace.HourlyValues.Select(x => x.IssueTypeId).ToList();

                    foreach (int i in Enumerable.Range(1, 23))
                    {
                        if (!existingIssueTypeIds.Contains((IssueType)i))
                        {
                            WorkspaceValuePerHour valuePerHour = new()
                            {
                                WorkspaceValuePerHourId = Guid.NewGuid(),
                                IssueType = (IssueType)i,
                                HourlyRate = 0,
                                WorkspaceId = newWorkspace.WorkspaceId
                            };

                            await workspaceValuePerHourRepository.AddValuePerHour(valuePerHour);
                        }
                    }

                    foreach (var value in workspace.HourlyValues)
                    {
                        WorkspaceValuePerHour valuePerHour = new()
                        {
                            WorkspaceValuePerHourId = Guid.NewGuid(),
                            IssueType = value.IssueTypeId,
                            HourlyRate = value.ValuePerHour,
                            WorkspaceId = newWorkspace.WorkspaceId
                        };

                        await workspaceValuePerHourRepository.AddValuePerHour(valuePerHour);
                    }


                    var valuesPerHour = await workspaceValuePerHourRepository.GetWorkspaceValuesPerHour(newWorkspace.WorkspaceId);
                    if (valuesPerHour.Count != 23) return Problem();
                }

                return Ok(new { id = newWorkspace.WorkspaceId });
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// List of accounts that belong provided workspace
        /// </summary>
        /// <response code="200">List of account details</response>
        /// <response code="500">Internal error</response>
        [HttpGet("workspace/{workspaceId}/accounts")]
        [Authorize]
        public async Task<IActionResult> GetAllAccountsForWorkspace(Guid workspaceId)
        {
            AccountRepository accountRepository = new();
            try
            {
                return Ok(await accountRepository.GetAccountsInWorkspace(workspaceId));
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// List of workspaces that authenticated account belong
        /// </summary>
        /// <response code="200">List of workspaces</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("workspace/all")]
        [Authorize]
        public async Task<IActionResult> GetAllWorkspacesForAccount()
        {
            AccountRepository accountRepository = new();
            WorkspaceRepository workspaceRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var workspaces = workspaceRepository.GetWorkspacesForAccountByAccountId(user.AccountId);

                JsonSerializerSettings settings = new()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(workspaces, settings);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides workspace details for a given workspace
        /// </summary>
        /// <response code="200">Workspace identification</response>
        /// <response code="400">Authenticated account does not belong to provided workspace</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("workspace/{workspaceId}")]
        [Authorize]
        public async Task<IActionResult> GetWorkspace(Guid workspaceId)
        {
            AccountRepository accountRepository = new();
            WorkspaceRepository workspaceRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var accountWorkspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(workspaceId, user.AccountId);
                if (accountWorkspace == null) return BadRequest(new { message = "User not member of provided workspace." });

                var workspace = await workspaceRepository.GetWorkspaceById(workspaceId);
                JsonSerializerSettings settings = new()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(workspace, settings);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Updates workspace roles for a given account
        /// </summary>
        /// <response code="200">Role updated</response>
        /// <response code="400">Authenticated account does not belong to provided workspace</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Not possible to update role</response>
        /// <response code="500">Internal error</response>
        [HttpPut("workspace/role/edit")]
        [Authorize(Policy = "RequireAdminOrProjManagerRole")]
        public async Task<IActionResult> UpdateRoleByIds(UpdateRoleRequest updateRoleRequest)
        {
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var accountWorkspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(updateRoleRequest.WorkspaceId, updateRoleRequest.AccountId);
                if (accountWorkspace == null) return BadRequest(new { message = "User not member of provided workspace." });

                accountWorkspace.RoleId = updateRoleRequest.RoleId;

                var updated = await accountWorkspaceRepository.UpdateAccountWorkspace(accountWorkspace);
                if (!updated) return Conflict(new { message = "Not possible to update role." });

                JsonSerializerSettings settings = new()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(accountWorkspace, settings);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Allows or denies hourly rates for workspace
        /// </summary>
        /// <response code="200">Workspace updated</response>
        /// <response code="400">Invalid workspace identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Not possible to update workspace preferences</response>
        /// <response code="500">Internal error</response>
        [HttpPost("workspace/hourly-price/use")]
        [Authorize(Policy = "RequireAdminOrProjManagerRole")]
        public async Task<IActionResult> UseHourlyPrice(UseHourlyPriceRequest useHourlyPriceRequest)
        {
            AccountRepository accountRepository = new();
            WorkspaceRepository workspaceRepository = new();
            WorkspaceValuePerHourRepository workspaceValuePerHourRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var workspace = await workspaceRepository.GetWorkspaceById(useHourlyPriceRequest.WorkspaceId);
                if (workspace == null) return BadRequest(new { message = "Workspace not found." });

                workspace.HasHourlyRate = useHourlyPriceRequest.AllowHourlyValues;
                if (useHourlyPriceRequest.AllowHourlyValues)
                {
                    workspace.Currency = useHourlyPriceRequest.Currency;
                }
                var result = await workspaceRepository.UpdateWorkspace(workspace);
                if (!result) return Conflict(new { message = "Not possible to update workspace preferences."});

                if (useHourlyPriceRequest.AllowHourlyValues)
                {
                    var workspaceValues = await workspaceValuePerHourRepository.GetWorkspaceValuesPerHour(useHourlyPriceRequest.WorkspaceId);
                    foreach (var workspaceValue in workspaceValues)
                    {
                        await workspaceValuePerHourRepository.DeleteWorkspaceValuePerHour(workspaceValue);
                    }

                    foreach (int i in Enumerable.Range(1, 23))
                    {
                        WorkspaceValuePerHour valuePerHour = new()
                        {
                            WorkspaceValuePerHourId = Guid.NewGuid(),
                            IssueType = (IssueType)i,
                            HourlyRate = 0,
                            WorkspaceId = useHourlyPriceRequest.WorkspaceId
                        };

                        await workspaceValuePerHourRepository.AddValuePerHour(valuePerHour);
                    }
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// List of hourly rates for workspace
        /// </summary>
        /// <response code="200">Workspace updated</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("workspace/{workspaceId}/hourly-price")]
        [Authorize]
        public async Task<IActionResult> GetHourlyPrices(Guid workspaceId)
        {
            AccountRepository accountRepository = new();
            WorkspaceValuePerHourRepository workspaceValuePerHourRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var valuesPerHour = await workspaceValuePerHourRepository.GetWorkspaceValuesPerHour(workspaceId);

                JsonSerializerSettings settings = new()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(valuesPerHour, settings);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Updates hourly rates for workspace
        /// </summary>
        /// <response code="200">List of Workspace hourly rates updated</response>
        /// <response code="400">Invalid workspace identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpPut("workspace/hourly-price/edit")]
        [Authorize]
        public async Task<IActionResult> EditHourlyPrices(UpdateHourlyPriceRequest updateHourlyPriceRequest)
        {
            AccountRepository accountRepository = new();
            WorkspaceValuePerHourRepository workspaceValuePerHourRepository = new();
            WorkspaceRepository workspaceRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var workspace = await workspaceRepository.GetWorkspaceById(updateHourlyPriceRequest.WorkspaceId);
                if (workspace == null) return BadRequest(new { message = "Workspace not found." });

                foreach (var item in updateHourlyPriceRequest.ValuesPerHour)
                {
                    var existing = await workspaceValuePerHourRepository.GetWorkspaceValuePerHourById(updateHourlyPriceRequest.WorkspaceId, item.WorkspaceValuePerHourId);
                    if (existing != null)
                    {
                        existing.HourlyRate = item.ValuePerHour;
                        await workspaceValuePerHourRepository.UpdateWorkspaceValuePerHour(existing);
                    }
                }

                var valuesPerHour = await workspaceValuePerHourRepository.GetWorkspaceValuesPerHour(updateHourlyPriceRequest.WorkspaceId);

                JsonSerializerSettings settings = new()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(valuesPerHour, settings);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides workspace role for a given workspace
        /// </summary>
        /// <response code="200">Workspace role for given account</response>
        /// <response code="400">Invalid workspace identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("workspace/{workspaceId}/role/{accountId}")]
        [Authorize]
        public async Task<IActionResult> GetRoleByIds(Guid workspaceId, Guid accountId)
        {
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var accountWorkspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(workspaceId, accountId);
                if (accountWorkspace == null) return BadRequest(new { message = "User not member of provided workspace." });

                JsonSerializerSettings settings = new()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(accountWorkspace, settings);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Adds accounts that belong same company to a given workspace
        /// </summary>
        /// <response code="200">Accounts added</response>
        /// <response code="204">No account added</response>
        /// <response code="400">Invalid workspace identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Invalid accounts</response>
        /// <response code="500">Internal error</response>
        [HttpPost("workspace/add-accounts")]
        [Authorize(Policy = "RequireAdminOrProjManagerRole")]
        public async Task<IActionResult> AddAccountsToWorkspace([FromBody] AddAccountWorkspaceRequest request)
        {
            AccountRepository accountRepository = new();
            WorkspaceRepository workspaceRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var workspace = await workspaceRepository.GetWorkspaceById(request.WorkspaceId);
                if (workspace == null) BadRequest(new { message = "Workspace not found." });

                List<Account> validAccounts = await accountRepository.GetAccountsByIds(request.AccountsToAdd);

                if (validAccounts.Count == 0) return Conflict(new { message = "No valid accounts to be added." });

                int counter = 0;
                EmailService emailService = new();

                foreach (var validAcc in validAccounts)
                {
                    var accountWorkspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(request.WorkspaceId, validAcc.AccountId);

                    AccountWorkspace repository = null;
                    if (accountWorkspace == null)
                    {
                        var accountToAdd = request.AccountsToAdd.Where(x => x.AccountId == validAcc.AccountId).FirstOrDefault();
                        var roleIdToAdd = accountToAdd?.RoleId ?? validAcc.RoleId;

                        counter++;
                        repository = new()
                        {
                            WorkspaceId = request.WorkspaceId,
                            AccountId = validAcc.AccountId,
                            AccountWorkspaceId = Guid.NewGuid(),
                            RoleId = roleIdToAdd
                        };

                        await accountWorkspaceRepository.AddAccountWorkspace(repository);

                        BackgroundJob.Enqueue(() => emailService.SendNewWorkspaceEmail(validAcc, workspace, email));
                    }
                }

                if (counter == 0) return NoContent();

                return Ok(counter == 1 ? "1 account added." : $"{counter} accounts added.");
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }
    }
}
