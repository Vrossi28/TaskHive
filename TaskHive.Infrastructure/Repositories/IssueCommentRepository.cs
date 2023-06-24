using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskHive.Core.Entities;
using TaskHive.Core.Utils;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class IssueCommentRepository
    {
        private TaskHiveContext _dbContext = new();

        private static Expression<Func<IssueComment, object>> GetSortProperty(string? sortColumn)
        {
            return (sortColumn?.ToLower()) switch
            {
                "comment" => issue => issue.Comment,
                "account" => issue => issue.AccountId,
                _ => issue => issue.IssueCommentId
            };
        }

        public async Task<IssueComment> GetCommentByIdAsync(Guid issueCommentId)
        {
            IssueComment issueComment = null;
            issueComment = await _dbContext.IssueComment.AsNoTracking().SingleOrDefaultAsync((e) => e.IssueCommentId == issueCommentId);

            return issueComment;
        }

        public async Task<PagedList<IssueCommentDto>> GetAllCommentsFromIssue(Guid issueId,
            string? searchTerm, string? sortColumn, string? sortOrder, int page,
            int pageSize)
        {
            IQueryable<IssueComment> query = _dbContext.IssueComment.AsNoTracking().Where((e) => e.IssueId == issueId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.Comment.Contains(searchTerm));

            if (sortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortColumn));
            }

            var comments = PagedList<IssueComment>.Create(query, page, pageSize);

            List <IssueCommentDto> dtos = new();

            foreach (var issueComment in comments.Items)
            {
                var dto = await ConvertToDto(issueComment);
                if (dto != null) dtos.Add(dto);
            }

            return new(dtos, comments.Page, comments.PageSize, comments.TotalCount);
        }

        public async Task<bool> AddCommentAsync(IssueComment issueComment)
        {
            _dbContext.IssueComment.Add(issueComment);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteCommentAsync(IssueComment issueComment)
        {
            _dbContext.IssueComment.Remove(issueComment);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<IssueCommentDto> ConvertToDto(IssueComment? comment)
        {
            IssueCommentDto dto = null;
            if (comment == null) return dto;

            AccountRepository accountRepository = new();
            var account = await accountRepository.GetAccountById(comment.AccountId);
            if (account == null) return dto;

            dto = new()
            {
                IssueCommentId = comment.IssueCommentId,
                AccountId = comment.AccountId,
                AccountEmail = account.Email,
                Comment = comment.Comment,
                IssueId = comment.IssueId
            };

            return dto;
        }
    }
}
