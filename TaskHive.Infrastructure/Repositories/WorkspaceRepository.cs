using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Core.Utils;
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

        public PagedList<Workspace> GetWorkspacesForAccountByAccountId(Guid accountId, 
            string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<AccountWorkspace> query = _dbContext.AccountWorkspace.AsNoTracking().Where((c) => c.AccountId == accountId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Join(_dbContext.Workspace,
                accountWorkspace => accountWorkspace.WorkspaceId,
                workspace => workspace.WorkspaceId,
                (accountWorkspace, workspace) => new { AccountWorkspace = accountWorkspace, Workspace = workspace })
                .Where(joinResult => joinResult.Workspace.Name.Contains(searchTerm))
                .Select(joinResult => joinResult.AccountWorkspace);

            if (sortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortColumn));
            }

            List<Workspace> workspaces = new();
            var accountWorkspaces = PagedList<AccountWorkspace>.Create(query, page, pageSize);

            foreach (var accountWorkspace in accountWorkspaces.Items)
            {
                var workspace = _dbContext.Workspace.AsNoTracking().Where((e) => e.WorkspaceId == accountWorkspace.WorkspaceId).FirstOrDefault();
                workspaces.Add(workspace);
            }

            return new(workspaces, accountWorkspaces.Page, accountWorkspaces.PageSize, accountWorkspaces.TotalCount);
        }

        public async Task<bool> AddWorkspace(Workspace workspace)
        {
            _dbContext.Workspace.Add(workspace);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        private static Expression<Func<AccountWorkspace, object>> GetSortProperty(string? sortColumn)
        {
            return (sortColumn?.ToLower()) switch
            {
                "role" => acc => acc.RoleId,
                "accountid" => acc => acc.AccountId,
                _ => work => work.AccountWorkspaceId
            };
        }
    }
}
