using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TaskHive.Application.Contracts.Requests
{
    public class UseHourlyPriceRequest
    {
        [DataMember(Name = "workspaceId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace id must be defined.")]
        public Guid WorkspaceId { get; set; }
        [DataMember(Name = "allowHourlyValues", IsRequired = true)]
        [Required(ErrorMessage = "Allow hourly values must be defined.")]
        public bool AllowHourlyValues { get; set; }
        [DataMember(Name = "currency", IsRequired = false)]
        [MaxLength(3, ErrorMessage = "Currency always have 3 digits.")]
        [MinLength(3, ErrorMessage = "Currency always have 3 digits.")]
        public string? Currency { get; set; }
    }
}
