using System.ComponentModel.DataAnnotations;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    public class EditIssueResolution
    {
        [Key]
        public Guid IssueResolutionId { get; set; }
        [Required]
        public Guid SolverAccountId { get; set; }
        [Required]
        public ResolutionType Type { get; set; }
        public string? Description { get; set; }
    }
}
