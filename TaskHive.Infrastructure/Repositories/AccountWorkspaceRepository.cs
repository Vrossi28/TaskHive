using Microsoft.EntityFrameworkCore;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class AccountWorkspaceRepository
    {
        private TaskHiveContext _dbContext = new();

        public AccountWorkspace GetAccountWorkspaceByIds(Guid workspaceId, Guid accountId)
        {
            AccountWorkspace accountWorkspace = null;

            accountWorkspace = _dbContext.AccountWorkspace.AsNoTracking().SingleOrDefault((a) => a.WorkspaceId == workspaceId && a.AccountId == accountId);

            return accountWorkspace;
        }

        public List<AccountWorkspace> GetAccountWorkspacesByWorkspaceId(Guid workspaceId)
        {
            List<AccountWorkspace> accountWorkspaces = null;

            accountWorkspaces = _dbContext.AccountWorkspace.AsNoTracking().Where((a) => a.WorkspaceId == workspaceId).ToList();

            return accountWorkspaces;
        }

        public async Task<bool> AddAccountWorkspace(AccountWorkspace accountWorkspace)
        {
            _dbContext.AccountWorkspace.Add(accountWorkspace);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<bool> UpdateAccountWorkspace(AccountWorkspace accountWorkspace)
        {
            _dbContext.AccountWorkspace.Update(accountWorkspace);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }
    }
}
