using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer.Inflections;
using Humanizer;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Persistence;
using TaskHive.Core.Entities;

namespace TaskHive.Infrastructure.Repositories
{
    public class IssueResolutionDetailRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<bool> AddIssueResolutionDetail(IssueResolution issueResolutionDetail)
        {
            _dbContext.IssueResolution.Add(issueResolutionDetail);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<IssueResolutionDetailDto> GetIssueResolutionDetailByIds(Guid issueId, Guid? issueResolutionDetailId)
        {
            IssueResolution issueResolutionDetail = null;
            IssueResolutionDetailDto dto = null;


            if (issueResolutionDetailId != null)
            {
                var issue = _dbContext.Issue.AsNoTracking().SingleOrDefault((a) => a.IssueResolutionId == issueResolutionDetailId && a.IssueId == issueId);
                if (issue != null)
                    issueResolutionDetail = _dbContext.IssueResolution.AsNoTracking().SingleOrDefault((a) => a.IssueResolutionId == issueResolutionDetailId);

                dto = await ConvertToDto(issueResolutionDetail);
            }

            return dto;
        }

        public async Task<bool> DeleteIssueResolutionDetail(IssueResolution issueResolutionDetail)
        {
            _dbContext.IssueResolution.Remove(issueResolutionDetail);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<IssueResolutionDetailDto> UpdateIssueResolutionDetailAsync(IssueResolution issueResolutionDetail, Guid issueId)
        {
            IssueResolution existingIssueResolutionDetail = await GetIssueResolutionDetailByIds(issueId, issueResolutionDetail.IssueResolutionId);
            IssueResolutionDetailDto dto = null;

            if (existingIssueResolutionDetail != null)
            {
                IssueResolution updatedIssueResolutionDetail = new()
                {
                    IssueResolutionId = issueResolutionDetail.IssueResolutionId,
                    Description = issueResolutionDetail.Description,
                    SolverAccountId = issueResolutionDetail.SolverAccountId,
                    Type = issueResolutionDetail.Type
                };

                _dbContext.IssueResolution.Update(updatedIssueResolutionDetail);
                var result = await _dbContext.SaveChangesAsync();
                if (Convert.ToBoolean(result))
                {
                    dto = await ConvertToDto(updatedIssueResolutionDetail);
                    return dto;
                }
            }
            return dto;
        }

        public async Task<IssueResolutionDetailDto> ConvertToDto(IssueResolution? issueResolutionDetail)
        {
            IssueResolutionDetailDto dto = null;
            if (issueResolutionDetail == null) return dto;

            AccountRepository accountRepository = new();
            var solverEmail = await accountRepository.GetAccountById(issueResolutionDetail.SolverAccountId);

            dto = new()
            {
                SolverAccountId = issueResolutionDetail.SolverAccountId,
                Description = issueResolutionDetail.Description,
                Type = issueResolutionDetail.Type,
                IssueResolutionId = issueResolutionDetail.IssueResolutionId,
                SolverEmail = solverEmail.Email,
                IssueResolutionDesc = issueResolutionDetail.Type.Humanize(),
            };

            return dto;
        }
    }
}
