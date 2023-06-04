using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;
using Newtonsoft.Json;

namespace TaskHive.Core.Entities
{
    public class WorkspaceValuePerHour
    {
        [Key]
        public Guid WorkspaceValuePerHourId { get; set; }
        [Required]
        public Guid WorkspaceId { get; set; }
        [Required]
        public IssueType IssueType { get; set; }
        [Required]
        public decimal HourlyRate { get; set; }

        [JsonIgnore]
        public virtual Workspace Workspace { get; set; }
    }
}
