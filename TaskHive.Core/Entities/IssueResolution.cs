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
    public class IssueResolution
    {
        [Key]
        public Guid IssueResolutionId { get; set; }
        [Required]
        public Guid SolverAccountId { get; set; }
        [Required]
        public ResolutionType Type { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public virtual Issue Issue { get; set; }
        [JsonIgnore]
        public virtual IssueResolutionDesc IssueResolutionDesc { get; set; }
    }
}
