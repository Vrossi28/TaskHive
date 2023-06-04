using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Infrastructure.Models
{
    public class AccountLoggedWorkDto
    {
        public Guid AccountLoggedWorkId { get; set; }
        public Guid AccountId { get; set; }
        public string AccountEmail { get; set; }
        public Guid IssueId { get; set; }
        public string IssueTitle { get; set; }
        public decimal TimeSpent { get; set; }
        public DateTime StartingDate { get; set; }
        public string? Description { get; set; }
        public decimal? EffortPrice { get; set; }
        public bool HasValuePerHour { get; set; }
    }
}
