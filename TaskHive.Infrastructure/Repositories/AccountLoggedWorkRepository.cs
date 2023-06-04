using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Core.Enums;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class AccountLoggedWorkRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<bool> AddLoggedWork(AccountLoggedWork loggedWork)
        {
            _dbContext.AccountLoggedWork.Add(loggedWork);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<AccountLoggedWork> GetLoggedWorkByIds(Guid issueId, Guid accountLoggedWorkId)
        {
            AccountLoggedWork accountLoggedWork = null;

            accountLoggedWork = await _dbContext.AccountLoggedWork.AsNoTracking().SingleOrDefaultAsync((e) => e.IssueId == issueId && e.AccountLoggedWorkId == accountLoggedWorkId);

            return accountLoggedWork;
        }

        public async Task<bool> DeleteLoggedWork(AccountLoggedWork accountLoggedWork)
        {
            _dbContext.AccountLoggedWork.Remove(accountLoggedWork);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<List<AccountLoggedWorkDto>> GetAllLoggedWorksForIssue(Guid issueId)
        {
            List<AccountLoggedWorkDto> result = new();

            var loggedWorks = _dbContext.AccountLoggedWork.AsNoTracking().Where((e) => e.IssueId == issueId).ToList();

            foreach (var loggedWork in loggedWorks)
            {
                result.Add(await ConvertToDto(loggedWork));
            }

            return result;
        }

        private async Task<AccountLoggedWorkDto> ConvertToDto(AccountLoggedWork loggedWork)
        {
            if (loggedWork == null) return new AccountLoggedWorkDto();

            Account account = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((e) => e.AccountId == loggedWork.AccountId);
            Issue issue = await _dbContext.Issue.AsNoTracking().SingleOrDefaultAsync((e) => e.IssueId == loggedWork.IssueId);
            WorkspaceRepository workspaceRepository = new();
            var usesValuePerHour = workspaceRepository.WorkspaceHasValuePerHour(issue?.WorkspaceId ?? Guid.NewGuid());


            AccountLoggedWorkDto dto = new()
            {
                AccountEmail = account?.Email,
                AccountId = loggedWork.AccountId,
                AccountLoggedWorkId = loggedWork.AccountLoggedWorkId,
                Description = loggedWork.Description,
                IssueId = loggedWork.IssueId,
                IssueTitle = issue?.Title,
                StartingDate = loggedWork.StartingDate,
                TimeSpent = loggedWork.TimeSpent,
                HasValuePerHour = usesValuePerHour,
            };

            if (usesValuePerHour)
            {
                dto.EffortPrice = await EffortPriceCalculationUtil.GetEffortPriceCalculated(issue?.WorkspaceId ?? Guid.NewGuid(),
                    issue?.Type ?? IssueType.Clarification, dto.TimeSpent);
            }

            return dto;
        }
    }
}
