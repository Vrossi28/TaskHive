using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;

namespace TaskHive.Infrastructure.Models
{
    public class IssueResolutionDetailDto : IssueResolution
    {
        public string SolverEmail { get; set; }
        public string IssueResolutionDesc { get; set; }
    }
}
