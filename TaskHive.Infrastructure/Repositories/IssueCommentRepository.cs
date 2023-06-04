using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class IssueCommentRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<IssueComment> GetCommentByIdAsync(Guid issueCommentId)
        {
            IssueComment issueComment = null;
            issueComment = await _dbContext.IssueComment.AsNoTracking().SingleOrDefaultAsync((e) => e.IssueCommentId == issueCommentId);

            return issueComment;
        }

        public async Task<List<IssueCommentDto>> GetAllCommentsFromIssue(Guid issueId)
        {
            List<IssueCommentDto> dtos = new();
            List<IssueComment> issueComments = new();
            issueComments = _dbContext.IssueComment.AsNoTracking().Where((e) => e.IssueId == issueId).ToList();

            if (issueComments.Count < 1) return dtos;

            foreach (var issueComment in issueComments)
            {
                var dto = await ConvertToDto(issueComment);
                if (dto != null) dtos.Add(dto);
            }

            return dtos;
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
