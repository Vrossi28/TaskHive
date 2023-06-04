using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;

namespace TaskHive.Core.Entities
{
    public class IssueResolutionDesc
    {
        [Key]
        public ResolutionType IssueResolutionId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        public virtual ICollection<IssueResolution> IssueResolutions { get; set; }
    }
}
