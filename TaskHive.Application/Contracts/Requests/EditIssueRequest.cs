using System.ComponentModel.DataAnnotations;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    public class EditIssueRequest
    {
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
    }
}
