using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class InviteRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<Invite> GetInviteByIdAsync(Guid inviteId)
        {
            Invite invite = null;
            invite = await _dbContext.Invite.AsNoTracking().SingleOrDefaultAsync((e) => e.InviteId == inviteId);

            return invite;
        }

        public async Task<InviteDto> GetInviteDtoById(Guid inviteId)
        {
            InviteDto dto = null;
            var invite = await _dbContext.Invite.AsNoTracking().SingleOrDefaultAsync((e) => e.InviteId == inviteId);
            if (invite != null)
            {
                WorkspaceRepository workspaceRepository = new();
                var workspace = await workspaceRepository.GetWorkspaceById(invite.WorkspaceId);

                CompanyRepository companyRepository = new();
                var company = await companyRepository.GetCompanyById(workspace.CompanyId);

                if (company != null)
                {
                    dto = new()
                    {
                        AccountId = invite.AccountId,
                        IsActive = invite.IsActive,
                        CompanyId = company.CompanyId,
                        CompanyName = company.Name,
                        ExpirationDate = invite.ExpirationDate,
                        InvitedEmail = invite.InvitedEmail,
                        InviteId = inviteId,
                        RoleId = invite.RoleId,
                        WorkspaceId = workspace.WorkspaceId,
                        WorkspaceName = workspace.Name
                    };
                }
            }
            return dto;
        }

        public async Task<bool> AddInvite(Invite invite)
        {
            _dbContext.Invite.Add(invite);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<bool> UpdateInvite(Invite invite)
        {
            _dbContext.Invite.Update(invite);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }
    }
}
