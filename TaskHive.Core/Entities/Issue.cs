using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace TaskHive.Core.Entities
{
    public class Issue
    {
        [Key]
        public Guid IssueId { get; set; }
        public Guid? ParentId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public Guid AssignedTo { get; set; }
        [Required]
        public Guid WorkspaceId { get; set; }
        [Required]
        public Guid CurrentAssignee { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public decimal? EstimatedTime { get; set; }
        [Required]
        public IssueType Type { get; set; }
        [Required]
        public PriorityType Priority { get; set; }
        [Required]
        public StatusType Status { get; set; }
        public Guid? IssueResolutionId { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }
        [JsonIgnore]
        public virtual ICollection<IssueFile> IssueFiles { get; set; }
        [JsonIgnore]
        public virtual ICollection<IssueComment> IssueComments { get; set; }
        [JsonIgnore]
        public virtual IssuePriorityDesc IssuePriorityDesc { get; set; }
        [JsonIgnore]
        public virtual IssueResolution IssueResolution { get; set; }
        [JsonIgnore]
        public virtual IssueStatusDesc IssueStatusDesc { get; set; }
        [JsonIgnore]
        public virtual IssueTypeDesc IssueTypeDesc { get; set; }
    }
}
