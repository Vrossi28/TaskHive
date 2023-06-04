using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Core.Enums;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class WorkspaceValuePerHourRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<bool> AddValuePerHour(WorkspaceValuePerHour valuePerHour)
        {
            _dbContext.WorkspaceValuePerHour.Add(valuePerHour);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<WorkspaceValuePerHour> GetWorkspaceValuePerHourById(Guid workspaceId, Guid workspaceValuePerHourId)
        {
            WorkspaceValuePerHour workspaceValuePerHour = null;

            workspaceValuePerHour = await _dbContext.WorkspaceValuePerHour.AsNoTracking().SingleOrDefaultAsync((e) => e.WorkspaceId == workspaceId && e.WorkspaceValuePerHourId == workspaceValuePerHourId);

            return workspaceValuePerHour;
        }

        public async Task<WorkspaceValuePerHour> GetWorkspaceValuePerHourByTypeId(Guid workspaceId, IssueType typeId)
        {
            WorkspaceValuePerHour workspaceValuePerHour = null;

            workspaceValuePerHour = await _dbContext.WorkspaceValuePerHour.AsNoTracking().SingleOrDefaultAsync((e) => e.WorkspaceId == workspaceId && e.IssueType == typeId);

            return workspaceValuePerHour;
        }

        public async Task<List<WorkspaceValuePerHour>> GetWorkspaceValuesPerHour(Guid workspaceId)
        {
            List<WorkspaceValuePerHour> workspaceValuePerHour = new();

            workspaceValuePerHour = _dbContext.WorkspaceValuePerHour.AsNoTracking().Where((e) => e.WorkspaceId == workspaceId).ToList();

            return workspaceValuePerHour;
        }

        public async Task<bool> DeleteWorkspaceValuePerHour(WorkspaceValuePerHour workspaceValuePerHour)
        {
            _dbContext.WorkspaceValuePerHour.Remove(workspaceValuePerHour);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<bool> UpdateWorkspaceValuePerHour(WorkspaceValuePerHour workspaceValuePerHour)
        {
            _dbContext.WorkspaceValuePerHour.Update(workspaceValuePerHour);

            int result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }
    }
}
