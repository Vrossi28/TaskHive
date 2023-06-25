using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Core.Enums;
using TaskHive.Core.Utils;
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

            accountLoggedWork = await _dbContext.AccountLoggedWork.
                AsNoTracking().
                SingleOrDefaultAsync((e) => e.IssueId == issueId && e.AccountLoggedWorkId == accountLoggedWorkId);

            return accountLoggedWork;
        }

        public async Task<bool> DeleteLoggedWork(AccountLoggedWork accountLoggedWork)
        {
            _dbContext.AccountLoggedWork.Remove(accountLoggedWork);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<PagedList<AccountLoggedWorkDto>> GetAllLoggedWorksForIssue(Guid issueId, string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<AccountLoggedWork> query = _dbContext.AccountLoggedWork.AsNoTracking().Where((e) => e.IssueId == issueId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.Description.Contains(searchTerm));

            if (sortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortColumn));
            }

            List<AccountLoggedWorkDto> result = new();

            var works = PagedList<AccountLoggedWork>.Create(query, page, pageSize);

            foreach (var loggedWork in works.Items)
            {
                result.Add(await ConvertToDto(loggedWork));
            }

            return new(result, works.Page, works.PageSize, works.TotalCount);
        }

        private static Expression<Func<AccountLoggedWork, object>> GetSortProperty(string? sortColumn)
        {
            return (sortColumn?.ToLower()) switch
            {
                "timespent" => work => work.TimeSpent,
                "accountid" => work => work.AccountId,
                "startedat" => work => work.StartingDate,
                "description" => work => work.Description,
                _ => work => work.AccountLoggedWorkId
            };
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
