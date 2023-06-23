using Amazon.Auth.AccessControlPolicy;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskHive.Application.Contracts.Requests;
using TaskHive.Application.Services.Email;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        /// <summary>
        /// Creates an invite and send register link to given email
        /// </summary>
        /// <response code="200">Invite identification</response>
        /// <response code="404">Authenticated account or workspace not found</response>
        /// <response code="500">Internal error</response>
        [HttpPost("invite")]
        [Authorize(Policy = "RequireAdminOrProjManagerRole")]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteRequest invite)
        {
            AccountRepository accountRepository = new();
            WorkspaceRepository workspaceRepository = new();
            InviteRepository inviteRepository = new();

            var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
            var user = await accountRepository.GetActiveAccountByEmailAsync(email);
            if (user == null) return NotFound(new { message = "User not found." });

            var workspace = await workspaceRepository.GetWorkspaceById(invite.WorkspaceId);
            if (workspace == null) return NotFound(new { message = "Workspace not found." });

            Invite newInvite = new()
            {
                AccountId = user.AccountId,
                InviteId = Guid.NewGuid(),
                IsActive = true,
                ExpirationDate = DateTime.Now.AddYears(1),
                InvitedEmail = invite.InvitedEmail,
                RoleId = invite.RoleId,
                WorkspaceId = invite.WorkspaceId
            };

            var inviteCreated = await inviteRepository.AddInvite(newInvite);
            if (!inviteCreated) return Problem();

            EmailService emailService = new();
            BackgroundJob.Enqueue(() => emailService.SendInvitationEmail(newInvite, user, workspace.Name));

            return Ok(new { token = newInvite.InviteId });
        }
    }
}
