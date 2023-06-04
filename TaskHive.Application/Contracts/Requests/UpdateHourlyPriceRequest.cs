using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using TaskHive.Infrastructure.Models;

namespace TaskHive.Application.Contracts.Requests
{
    public class UpdateHourlyPriceRequest
    {
        [DataMember(Name = "workspaceId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace id must be defined.")]
        public Guid WorkspaceId { get; set; }

        [DataMember(Name = "valuesPerHour", IsRequired = true)]
        [Required(ErrorMessage = "Values per hour list must be defined.")]
        public List<EditWorkspaceValuePerHourItem> ValuesPerHour { get; set; }
    }
}
