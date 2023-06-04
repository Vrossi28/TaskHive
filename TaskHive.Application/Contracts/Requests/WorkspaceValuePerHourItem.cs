using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    public class EditWorkspaceValuePerHourItem
    {
        [DataMember(Name = "workspaceValuePerHourId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace value per hour id must be defined.")]
        public Guid WorkspaceValuePerHourId { get; set; }
        [DataMember(Name = "valuePerHour", IsRequired = true)]
        [Required(ErrorMessage = "Value per hour must be defined.")]
        public decimal ValuePerHour { get; set; }
    }
}
