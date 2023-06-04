using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;

namespace TaskHive.Infrastructure.Models
{
    public class IssueDto : Issue
    {
        public decimal? TimeSpent { get; set; }
        public string? ParentName { get; set; }
        public string CreatedByEmail { get; set; }
        public string AssignedToEmail { get; set; }
        public string CurrentAssigneeEmail { get; set; }
        public string WorkspaceName { get; set; }
        public string IssueTypeDesc { get; set; }
        public string PriorityDesc { get; set; }
        public string IssueStatusDesc { get; set; }
        public bool HasValuePerHour { get; set; }
        public decimal? EffortPrice { get; set; }
    }
}
