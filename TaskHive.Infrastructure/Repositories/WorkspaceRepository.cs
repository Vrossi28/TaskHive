using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class WorkspaceRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<bool> UpdateWorkspace(Workspace workspace)
        {
            _dbContext.Workspace.Update(workspace);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<Workspace> GetWorkspaceById(Guid workspaceId)
        {
            Workspace workspace = null;
            workspace = await _dbContext.Workspace.AsNoTracking().SingleOrDefaultAsync((w) => w.WorkspaceId == workspaceId);

            return workspace;
        }

        public bool WorkspaceHasValuePerHour(Guid workspaceId)
        {
            return _dbContext.Workspace.Where(x => x.WorkspaceId == workspaceId).Select(x => x.HasHourlyRate).FirstOrDefault();
        }

        public List<Workspace> GetWorkspacesByCompanyId(Guid companyId)
        {
            List<Workspace> workspaces = null;
            workspaces = _dbContext.Workspace.AsNoTracking().Where((c) => c.CompanyId == companyId).ToList();

            return workspaces;
        }

        public List<Workspace> GetWorkspacesForAccountByAccountId(Guid accountId)
        {
            List<Workspace> workspaces = new();
            List<AccountWorkspace> accountWorkspaces = _dbContext.AccountWorkspace.AsNoTracking().Where((c) => c.AccountId == accountId).ToList();

            foreach (var accountWorkspace in accountWorkspaces)
            {
                var workspace = _dbContext.Workspace.AsNoTracking().Where((e) => e.WorkspaceId == accountWorkspace.WorkspaceId).FirstOrDefault();
                workspaces.Add(workspace);
            }

            return workspaces;
        }

        public async Task<bool> AddWorkspace(Workspace workspace)
        {
            _dbContext.Workspace.Add(workspace);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }
    }
}
