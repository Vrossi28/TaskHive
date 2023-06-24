using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Core.Enums;
using TaskHive.Core.Utils;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class IssueRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<IssueDto> GetIssueByIdAsync(Guid? issueId)
        {
            IssueDto issueDto = null;
            try
            {
                if (issueId == null)
                    return issueDto;

                Issue issue = await _dbContext.Issue.AsNoTracking().SingleOrDefaultAsync((e) => e.IssueId == issueId);
                if(issue != null)
                    issueDto = await ConvertToDto(issue);

                return issueDto;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return issueDto;
            }
        }

        public async Task<IssueDto> UpdateIssueAsync(Issue issue)
        {
            Issue updatedIssue = null;
            var existingIssue = await _dbContext.Issue.AsNoTracking().SingleOrDefaultAsync(e => e.IssueId == issue.IssueId);
            if (existingIssue != null)
            {
                updatedIssue = new()
                {
                    IssueId = issue.IssueId,
                    ParentId = issue.ParentId,
                    Title = issue.Title,
                    Description = issue.Description,
                    CreatedBy = issue.CreatedBy,
                    AssignedTo = issue.AssignedTo,
                    WorkspaceId = issue.WorkspaceId,
                    CurrentAssignee = issue.CurrentAssignee,
                    CreatedAt = issue.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    EstimatedTime = issue.EstimatedTime,
                    Type = issue.Type,
                    Priority = issue.Priority,
                    Status = issue.Status,
                    IssueResolutionId = issue.IssueResolutionId,
                };

                _dbContext.Issue.Update(updatedIssue);
                var result = await _dbContext.SaveChangesAsync();
                if (Convert.ToBoolean(result))
                {
                    return await ConvertToDto(updatedIssue);
                }
            }
            return new IssueDto();
        }

        public async Task<PagedList<IssueDto>> GetIssuesForAccountByAccountId(Guid accountId, 
            string? searchTerm, Guid? workspaceId, string? sortColumn, string? sortOrder, int page, 
            int pageSize)
        {
            IQueryable<Issue> query = _dbContext.Issue.Where(i => i.CurrentAssignee == accountId);

            if (!string.IsNullOrEmpty(searchTerm)) 
                query = query.Where(p => p.Description.Contains(searchTerm) || p.Title.Contains(searchTerm));

            if (workspaceId is not null)
                query = query.Where(p => p.WorkspaceId == workspaceId);

            if (sortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortColumn));
            }

            var issues = PagedList<Issue>.Create(query, page, pageSize);

            List<IssueDto> detailedIssues = await GetDetailedIssues(issues.Items);

            return new(detailedIssues, issues.Page, issues.PageSize, issues.TotalCount);
        }

        public async Task<PagedList<IssueDto>> GetIssuesForWorkspace(Guid workspaceId, 
            string searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<Issue> query = _dbContext.Issue.Where(i => i.WorkspaceId == workspaceId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.Description.Contains(searchTerm) || p.Title.Contains(searchTerm));

            if (sortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortColumn));
            }

            var issues = PagedList<Issue>.Create(query, page, pageSize);

            List<IssueDto> detailedIssues = await GetDetailedIssues(issues.Items);

            return new(detailedIssues, issues.Page, issues.PageSize, issues.TotalCount);
        }

        public async Task<PagedList<IssueDto>> GetChildIssues(Guid issueId,
            string? searchTerm, Guid? workspaceId, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<Issue> query = _dbContext.Issue.Where(i => i.ParentId == issueId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.Description.Contains(searchTerm) || p.Title.Contains(searchTerm));

            if (workspaceId is not null)
                query = query.Where(p => p.WorkspaceId == workspaceId);

            if (sortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortColumn));
            }

            var issues = PagedList<Issue>.Create(query, page, pageSize);

            List<IssueDto> detailedIssues = await GetDetailedIssues(issues.Items);

            return new(detailedIssues, issues.Page, issues.PageSize, issues.TotalCount);
        }

        public async Task<bool> AddIssue(Issue issue)
        {
            try
            {
                _dbContext.Issue.Add(issue);

                int result = await _dbContext.SaveChangesAsync();

                return Convert.ToBoolean(result);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateIssue(Issue issue)
        {
            _dbContext.Issue.Update(issue);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        private async Task<List<IssueDto>> GetDetailedIssues(List<Issue> issues)
        {
            List<IssueDto> detailedIssues = new();

            foreach (var issue in issues)
            {
                IssueDto issueDto = await ConvertToDto(issue);

                detailedIssues.Add(issueDto);
            }

            return detailedIssues;
        }

        private async Task<IssueDto> ConvertToDto(Issue? issue)
        {
            if (issue == null) return new IssueDto();
            Issue? parent = await _dbContext.Issue.AsNoTracking().SingleOrDefaultAsync((e) => e.IssueId == issue.ParentId);
            WorkspaceRepository workspaceRepository = new();
            List<AccountLoggedWork> loggedWorks = _dbContext.AccountLoggedWork.AsNoTracking().Where((e) => e.IssueId == issue.IssueId).ToList();
            Workspace workspace = await _dbContext.Workspace.AsNoTracking().SingleOrDefaultAsync((e) => e.WorkspaceId == issue.WorkspaceId);
            Account createdBy = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((e) => e.AccountId == issue.CreatedBy);
            Account assignedTo = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((e) => e.AccountId == issue.AssignedTo);
            Account currentAssignee = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((e) => e.AccountId == issue.CurrentAssignee);
            string? issueTypeDesc = Enum.GetName(typeof(IssueType), issue.Type);
            string? priorityDesc = Enum.GetName(typeof(PriorityType), issue.Priority);
            var usesValuePerHour = workspaceRepository.WorkspaceHasValuePerHour(issue?.WorkspaceId ?? Guid.NewGuid());

            IssueDto dto = new()
            {
                AssignedTo = issue.AssignedTo,
                CreatedAt = issue.CreatedAt,
                CurrentAssignee = issue.CurrentAssignee,
                UpdatedAt = issue.UpdatedAt,
                CreatedBy = issue.CreatedBy,
                Description = issue.Description,
                EstimatedTime = issue.EstimatedTime,
                IssueId = issue.IssueId,
                Type = issue.Type,
                ParentId = issue.ParentId,
                Priority = issue.Priority,
                Title = issue.Title,
                WorkspaceId = issue.WorkspaceId,
                TimeSpent = (from a in loggedWorks select a.TimeSpent).Sum(),
                WorkspaceName = workspace?.Name,
                ParentName = parent?.Title,
                CreatedByEmail = createdBy?.Email,
                AssignedToEmail = assignedTo?.Email,
                CurrentAssigneeEmail = currentAssignee?.Email,
                IssueTypeDesc = issue.Type.Humanize(),
                PriorityDesc = priorityDesc,
                IssueResolutionId = issue.IssueResolutionId,
                Status = issue.Status,
                IssueStatusDesc = issue.Status.Humanize(),
                HasValuePerHour = usesValuePerHour
            };

            if (usesValuePerHour)
            {
                dto.EffortPrice = await EffortPriceCalculationUtil.GetEffortPriceCalculated(issue.WorkspaceId, issue.Type, dto.TimeSpent ?? 0);
            }

            return dto;
        }

        private static Expression<Func<Issue, object>> GetSortProperty(string? sortColumn)
        {
            return (sortColumn?.ToLower()) switch
            {
                "title" => issue => issue.Title,
                "status" => issue => issue.Status,
                "createdat" => issue => issue.CreatedAt,
                "updatedat" => issue => issue.UpdatedAt,
                _ => issue => issue.IssueId
            };
        }
    }
}
