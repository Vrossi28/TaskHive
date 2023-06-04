using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskHive.Core.Entities
{
    public class IssueComment
    {
        [Key]
        public Guid IssueCommentId { get; set; }

        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public Guid IssueId { get; set; }
        [Required]
        public string Comment { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }
        [JsonIgnore]
        public virtual Issue Issue { get; set; }
    }
}
