using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Core.Enums;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.Infrastructure
{
    internal class EffortPriceCalculationUtil
    {
        public static async Task<decimal> GetEffortPriceCalculated(Guid workspaceId, IssueType issueTypeId, decimal timeSpent)
        {
            WorkspaceValuePerHourRepository workspaceValuePerHourRepository = new();
            var valuePerHour = await workspaceValuePerHourRepository.GetWorkspaceValuePerHourByTypeId(workspaceId, issueTypeId);
            if (valuePerHour == null) return 0;

            decimal hours = Math.Round(timeSpent / 60, 2);
            return valuePerHour.HourlyRate * hours;
        }
    }
}
