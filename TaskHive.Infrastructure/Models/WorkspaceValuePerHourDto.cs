using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using TaskHive.Core.Enums;

namespace TaskHive.Infrastructure.Models
{
    public class WorkspaceValuePerHourDto
    {
        [DataMember(Name = "workspaceValuePerHourId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace value per hour id must be defined.")]
        public Guid WorkspaceValuePerHourId { get; set; }

        [DataMember(Name = "workspaceId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace id must be defined.")]
        public Guid WorkspaceId { get; set; }

        [DataMember(Name = "issueTypeId", IsRequired = true)]
        [Required(ErrorMessage = "Issue type id must be defined.")]
        public IssueType IssueTypeId { get; set; }
        [DataMember(Name = "valuePerHour", IsRequired = true)]
        [Required(ErrorMessage = "Value per hour must be defined.")]
        public decimal ValuePerHour { get; set; }
    }
}
