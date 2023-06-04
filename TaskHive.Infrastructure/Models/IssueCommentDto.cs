using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Infrastructure.Models
{
    public class IssueCommentDto
    {
        public Guid IssueCommentId { get; set; }
        public Guid AccountId { get; set; }
        public string AccountEmail { get; set; }
        public Guid IssueId { get; set; }
        public string Comment { get; set; }
    }
}
