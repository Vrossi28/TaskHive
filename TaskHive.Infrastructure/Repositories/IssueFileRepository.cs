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
    public class IssueFileRepository
    {
        private TaskHiveContext _dbContext = new();

        public List<IssueFile> GetFilesByIssueId(Guid issueId)
        {
            List<IssueFile> files = new();

            files = _dbContext.IssueFile.AsNoTracking().Where((e) => e.IssueId == issueId).ToList();

            return files;
        }

        public async Task<IssueFile> GetFileByIds(Guid issueId, Guid issueFileId)
        {
            IssueFile file = null;

            file = await _dbContext.IssueFile.AsNoTracking().SingleOrDefaultAsync((e) => e.IssueId == issueId && e.IssueFileId == issueFileId);

            return file;
        }

        public async Task<bool> AddFile(IssueFile issueFile)
        {
            _dbContext.IssueFile.Add(issueFile);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<bool> UpdateIssueFile(IssueFile issueFile)
        {
            _dbContext.IssueFile.Update(issueFile);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteIssueFile(IssueFile issueFile)
        {
            _dbContext.IssueFile.Remove(issueFile);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }
    }
}
