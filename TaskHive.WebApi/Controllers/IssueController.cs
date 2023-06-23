using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using TaskHive.Application.Contracts.Requests;
using TaskHive.Application.Services.Email;
using TaskHive.Application.Services.SignalR;
using TaskHive.Core.Entities;
using TaskHive.Core.Enums;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private ISignalRContract _signalRContract;
        public IssueController(ISignalRContract signalRContract)
        {
            _signalRContract = signalRContract;
        }

        /// <summary>
        /// Creates an issue and sends email to involved accounts
        /// </summary>
        /// <response code="200">Token</response>
        /// <response code="400">Assigned user not found</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Assigned account is not in defined workspace</response>
        /// <response code="500">Internal error</response>
        [HttpPost("issue")]
        [Authorize(Policy = "RequireAdminProjManagerOrTeamMemberRole")]
        public async Task<IActionResult> AddIssue([FromBody] AddIssueRequest addIssue)
        {
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            IssueRepository issueRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var assignedUser = await accountRepository.GetAccountById(addIssue.AssignedToAccId);
                if (assignedUser == null) return BadRequest(new { message = "Assigned user not found." });

                var accountWorkspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(addIssue.WorkspaceId, addIssue.AssignedToAccId);
                if (accountWorkspace == null) return Conflict(new { message = "Assigned account is not in defined workspace." });

                var userAccountWorkspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(addIssue.WorkspaceId, user.AccountId);
                if (userAccountWorkspace == null) return Conflict(new { message = "User cannot assign a new issue if is not in the desired workspace." });

                if (addIssue.ParentId != null)
                {
                    var parent = await issueRepository.GetIssueByIdAsync(addIssue.ParentId);
                    if (parent.WorkspaceId != addIssue.WorkspaceId)
                        return Conflict(new { message = "Parent issue is from another workspace." });
                }

                Issue newIssue = new()
                {
                    AssignedTo = addIssue.AssignedToAccId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CurrentAssignee = addIssue.AssignedToAccId,
                    Description = addIssue.Description,
                    IssueId = Guid.NewGuid(),
                    Title = addIssue.Title,
                    WorkspaceId = addIssue.WorkspaceId,
                    CreatedBy = user.AccountId,
                    ParentId = addIssue.ParentId,
                    Priority = addIssue.PriorityId,
                    Type = addIssue.IssueTypeId,
                    EstimatedTime = addIssue.EstimatedTime,
                    Status = StatusType.Open
                };

                bool issueCreated = await issueRepository.AddIssue(newIssue);
                if (!issueCreated) return Problem();

                if (user.Email != assignedUser.Email) 
                {
                    EmailService emailService = new();
                    BackgroundJob.Enqueue(() => emailService.SendTaskAssignedEmail(assignedUser, newIssue, email)); 
                }

                await _signalRContract.UpdateIssue(user.AccountId.ToString(), newIssue.IssueId);

                return Ok(new { id = newIssue.IssueId });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// List of issues assigned to authenticated account
        /// </summary>
        /// <response code="200">List of issues</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/for-account/all")]
        [Authorize]
        public async Task<IActionResult> GetAllIssuesForAccount()
        {
            AccountRepository accountRepository = new();
            IssueRepository issueRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issues = await issueRepository.GetIssuesForAccountByAccountId(user.AccountId);
                var json = JsonConvert.SerializeObject(issues, Formatting.Indented);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides issue details
        /// </summary>
        /// <response code="200">Issue details</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/{issueId}")]
        [Authorize]
        public async Task<IActionResult> GetIssueById(Guid issueId)
        {
            AccountRepository accountRepository = new();
            IssueRepository issueRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Provided issue was not found" });
                var json = JsonConvert.SerializeObject(issue, Formatting.Indented);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides list of issue details for given worskspace
        /// </summary>
        /// <response code="200">List of issues</response>
        /// <response code="400">Account does not belong to given workspace</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/for-workspace/{workspaceId}")]
        [Authorize]
        public async Task<IActionResult> GetAllIssuesForWorkspace(Guid workspaceId)
        {
            AccountRepository accountRepository = new();
            IssueRepository issueRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var accWorkspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(workspaceId, user.AccountId);
                if (user == null) return BadRequest(new { message = "User not member of defined workspace." });

                var issues = await issueRepository.GetIssuesForWorkspace(workspaceId);
                var json = JsonConvert.SerializeObject(issues, Formatting.Indented);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides list of child issue details for given issue
        /// </summary>
        /// <response code="200">List of issues</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/{issueId}/childs")]
        [Authorize]
        public async Task<IActionResult> GetChildsForIssue(Guid issueId)
        {
            IssueRepository issueRepository = new();
            try
            {
                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return NotFound(new { message = "Provided issue was not found" });

                var issues = await issueRepository.GetChildIssues(issueId);
                var json = JsonConvert.SerializeObject(issues, Formatting.Indented);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides list of issues for account from given worskspace
        /// </summary>
        /// <response code="200">List of issues</response>
        /// <response code="400">Invalid workspace identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/for-account/{workspaceId}")]
        [Authorize]
        public async Task<IActionResult> GetAllIssuesForAccountByWorkspaceId(Guid workspaceId)
        {
            AccountRepository accountRepository = new();
            IssueRepository issueRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(workspaceId, user.AccountId);
                if (workspace == null) return BadRequest(new { message = "Workspace not found." });

                var issues = await issueRepository.GetIssuesForAccountByIds(user.AccountId, workspaceId);
                var json = JsonConvert.SerializeObject(issues, Formatting.Indented);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Creates a work log for an issue
        /// </summary>
        /// <response code="200">Logged work identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpPost("issue/log-work")]
        [Authorize]
        public async Task<IActionResult> LogWork(LogWorkRequest logWork)
        {
            AccountRepository accountRepository = new();
            AccountLoggedWorkRepository accountLoggedWorkRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                AccountLoggedWork loggedWork = new()
                {
                    AccountLoggedWorkId = Guid.NewGuid(),
                    AccountId = user.AccountId,
                    Description = logWork.Description,
                    IssueId = logWork.IssueId,
                    StartingDate = logWork.StartingDate,
                    TimeSpent = logWork.TimeSpent
                };

                var result = await accountLoggedWorkRepository.AddLoggedWork(loggedWork);
                if (!result) return Problem();

                await _signalRContract.UpdateIssue(user.AccountId.ToString(), logWork.IssueId);

                return Ok(new { id = loggedWork.AccountLoggedWorkId });
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a work log
        /// </summary>
        /// <response code="204">Deleted</response>
        /// <response code="404">Logged work not found</response>
        /// <response code="500">Internal error</response>
        [HttpDelete("issue/{issueId}/log-work/{accountLoggedWorkId}")]
        public async Task<IActionResult> DeleteLoggedWork(Guid issueId, Guid accountLoggedWorkId)
        {
            var accountLoggedWorkRepository = new AccountLoggedWorkRepository();

            var existing = await accountLoggedWorkRepository.GetLoggedWorkByIds(issueId, accountLoggedWorkId);

            if (existing != null)
            {
                var deleted = await accountLoggedWorkRepository.DeleteLoggedWork(existing);
                if (deleted)
                {
                    await _signalRContract.UpdateIssue(existing.AccountId.ToString(), existing.IssueId);
                    return NoContent();
                }
                return Problem();
            }

            return NotFound();
        }

        /// <summary>
        /// List of all logged works for a given issue
        /// </summary>
        /// <response code="200">List of logged works</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/{issueId}/log-work")]
        [Authorize]
        public async Task<IActionResult> TimeLoggedForIssue(Guid issueId)
        {
            AccountRepository accountRepository = new();
            AccountLoggedWorkRepository accountLoggedWorkRepository = new();
            IssueRepository issueRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue.IssueId != issueId) return BadRequest(new { message = "Issue not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                var result = await accountLoggedWorkRepository.GetAllLoggedWorksForIssue(issueId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Updates an issue using provided issue details
        /// </summary>
        /// <response code="200">Issue identification</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpPut("issue/edit")]
        [Authorize]
        public async Task<IActionResult> UpdateIssue(EditIssueRequest issue)
        {
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            IssueRepository issueRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var assignedUser = await accountRepository.GetAccountById(issue.CurrentAssignee);
                if (assignedUser == null) return BadRequest(new { message = "Assigned user not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                var issues = await issueRepository.GetIssuesForWorkspace(issue.WorkspaceId);
                var existingIssue = issues.FirstOrDefault(p => p.IssueId == issue.IssueId);
                if (existingIssue == null) return BadRequest(new { message = "Issue not found." });

                var prevAssigned = await accountRepository.GetAccountById(existingIssue.CurrentAssignee);

                var mapped = ConvertToIssueObj(issue);

                var result = await issueRepository.UpdateIssueAsync(mapped);
                if (result.IssueId != issue.IssueId) return Problem();

                if (prevAssigned.Email != assignedUser.Email)
                {
                    EmailService emailService = new();
                    BackgroundJob.Enqueue(() => emailService.SendTaskAssignedEmail(assignedUser, mapped, email));
                }
                await _signalRContract.UpdateIssue(user.AccountId.ToString(), result.IssueId);

                return Ok(new { id = result.IssueId });
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Closes an issue and sends an email to involved accounts
        /// </summary>
        /// <response code="200">Issue identification</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpPost("issue/{issueId}/resolution")]
        [Authorize]
        public async Task<IActionResult> CloseIssue(CloseIssueRequest issueResolutionDetailRequest, Guid issueId)
        {
            IssueResolutionDetailRepository issueResolutionDetailRepository = new();
            IssueRepository issueRepository = new();
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Issue not found." });
                if (issue.IssueResolutionId != null) return NoContent();

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });


                IssueResolution issueResolutionDetail = new()
                {
                    IssueResolutionId = Guid.NewGuid(),
                    Description = issueResolutionDetailRequest.Description,
                    Type = issueResolutionDetailRequest.IssueResolutionId,
                    SolverAccountId = user.AccountId
                };

                var result = await issueResolutionDetailRepository.AddIssueResolutionDetail(issueResolutionDetail);
                if (!result) return Problem();
                issue.IssueResolutionId = issueResolutionDetail.IssueResolutionId;
                issue.Status = StatusType.Closed;

                var issueResult = await issueRepository.UpdateIssue(issue);
                if (!issueResult) return Problem();

                EmailService emailService = new();
                BackgroundJob.Enqueue(() => emailService.SendTaskClosedEmail(issue, issueResolutionDetail));
                await _signalRContract.UpdateIssue(user.AccountId.ToString(), issue.IssueId);

                return Ok(new { id = issueResolutionDetail.IssueResolutionId });
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Updates an issue resolution
        /// </summary>
        /// <response code="200">Issue resolution updated</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpPut("issue/{issueId}/resolution")]
        [Authorize]
        public async Task<IActionResult> UpdateResolution(EditIssueResolution issueResolution, Guid issueId)
        {
            IssueResolutionDetailRepository issueResolutionDetailRepository = new();
            IssueRepository issueRepository = new();
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Issue not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                var mapped = ConvertToIssueResolutionObj(issueResolution);

                var result = await issueResolutionDetailRepository.UpdateIssueResolutionDetailAsync(mapped, issueId);
                if (result == null) return Problem();

                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                await _signalRContract.UpdateIssue(user.AccountId.ToString(), issueId);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Reopen an issue
        /// </summary>
        /// <response code="204">Issue reopened</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpPut("issue/{issueId}/reopen")]
        [Authorize]
        public async Task<IActionResult> ReopenIssue(Guid issueId)
        {
            IssueResolutionDetailRepository issueResolutionDetailRepository = new();
            IssueRepository issueRepository = new();
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Issue not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                var issueResolutionDetail = await issueResolutionDetailRepository.GetIssueResolutionDetailByIds(issue.IssueId, issue.IssueResolutionId);
                if (issueResolutionDetail == null) return BadRequest(new { message = "This issue does not have a resolution." });

                issue.IssueResolutionId = null;
                issue.Status = StatusType.Open;
                var updated = await issueRepository.UpdateIssue(issue);
                if (updated)
                {
                    await issueResolutionDetailRepository.DeleteIssueResolutionDetail(issueResolutionDetail);
                    await _signalRContract.UpdateIssue(user.AccountId.ToString(), issueId);
                    return NoContent();
                }
                return Problem();
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides a comment for an issue
        /// </summary>
        /// <response code="200">Comment details</response>
        /// <response code="400">Invalid comment identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/{issueId}/comment/{issueCommentId}")]
        [Authorize]
        public async Task<IActionResult> GetCommentById(Guid issueId, Guid issueCommentId)
        {
            IssueRepository issueRepository = new();
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            IssueCommentRepository issueCommentRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Issue not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                var existingComment = await issueCommentRepository.GetCommentByIdAsync(issueCommentId);
                if (existingComment == null) return NotFound(new { message = "Comment not found." });

                var dto = await issueCommentRepository.ConvertToDto(existingComment);
                if (dto == null) return NotFound(new { message = "Account not found." });

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Provides list of comments for an issue
        /// </summary>
        /// <response code="200">List of comment details</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpGet("issue/{issueId}/comment")]
        [Authorize]
        public async Task<IActionResult> GetAllCommentsByIssue(Guid issueId)
        {
            IssueRepository issueRepository = new();
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            IssueCommentRepository issueCommentRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Issue not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                var existingComments = await issueCommentRepository.GetAllCommentsFromIssue(issueId);

                return Ok(existingComments);
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new comment for an issue
        /// </summary>
        /// <response code="200">Comment identification</response>
        /// <response code="400">Invalid issue identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpPost("issue/{issueId}/comment")]
        [Authorize]
        public async Task<IActionResult> AddComment(Guid issueId, AddCommentRequest comment)
        {
            IssueRepository issueRepository = new();
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            IssueCommentRepository issueCommentRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Issue not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                IssueComment newComment = new()
                {
                    IssueCommentId = Guid.NewGuid(),
                    IssueId = issueId,
                    AccountId = comment.AccountId,
                    Comment = comment.Comment
                };

                var saved = await issueCommentRepository.AddCommentAsync(newComment);
                if (!saved) return Problem();

                await _signalRContract.UpdateIssue(user.AccountId.ToString(), issueId);
                return Ok(new { id = newComment.IssueCommentId });
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a comment for an issue
        /// </summary>
        /// <response code="204">Comment deleted</response>
        /// <response code="400">Invalid comment identification</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Account is not a member of this workspace</response>
        /// <response code="500">Internal error</response>
        [HttpDelete("issue/{issueId}/comment/{issueCommentId}")]
        [Authorize]
        public async Task<IActionResult> RemoveComment(Guid issueId, Guid issueCommentId)
        {
            IssueRepository issueRepository = new();
            AccountRepository accountRepository = new();
            AccountWorkspaceRepository accountWorkspaceRepository = new();
            IssueCommentRepository issueCommentRepository = new();

            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "User not found." });

                var issue = await issueRepository.GetIssueByIdAsync(issueId);
                if (issue == null) return BadRequest(new { message = "Issue not found." });

                var workspace = accountWorkspaceRepository.GetAccountWorkspaceByIds(issue.WorkspaceId, user.AccountId);
                if (workspace == null) return Conflict(new { message = "User is not a member of this workspace." });

                var existingComment = await issueCommentRepository.GetCommentByIdAsync(issueCommentId);
                if (existingComment == null) return NotFound(new { message = "Comment not found." });

                var deleted = await issueCommentRepository.DeleteCommentAsync(existingComment);
                if (!deleted) return Problem();

                await _signalRContract.UpdateIssue(user.AccountId.ToString(), issueId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }

        private Issue ConvertToIssueObj(EditIssueRequest request)
        {
            Issue issue = new()
            {
                IssueId = request.IssueId,
                ParentId = request.ParentId,
                Title = request.Title,
                Description = request.Description,
                CreatedBy = request.CreatedBy,
                AssignedTo = request.AssignedTo,
                WorkspaceId = request.WorkspaceId,
                CurrentAssignee = request.CurrentAssignee,
                CreatedAt = request.CreatedAt,
                EstimatedDelivery = request.EstimatedDelivery,
                EstimatedTime = request.EstimatedTime,
                Type = request.Type,
                Priority = request.Priority,
                Status = request.Status,
                IssueResolutionId = request.IssueResolutionId
            };

            return issue;
        }

        private IssueResolution ConvertToIssueResolutionObj(EditIssueResolution request)
        {
            IssueResolution resolution = new()
            {
                IssueResolutionId = request.IssueResolutionId,
                SolverAccountId = request.SolverAccountId,
                Type = request.Type,
                Description = request.Description
            };

            return resolution;
        }
    }
}
