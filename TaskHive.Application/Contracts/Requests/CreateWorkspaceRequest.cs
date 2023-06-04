using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TaskHive.Infrastructure.Models;

namespace TaskHive.Application.Contracts.Requests
{
    public class CreateWorkspaceRequest
    {
        [DataMember(Name = "workspaceName", IsRequired = true)]
        [Required(ErrorMessage = "Workspace name must be defined.")]
        [MaxLength(30, ErrorMessage = "Workspace name cannot have more than 30 digits.")]
        [MinLength(2, ErrorMessage = "Workspace name cannot have less than 2 digits.")]
        public string WorkspaceName { get; set; }

        [DataMember(Name = "shortName", IsRequired = true)]
        [Required(ErrorMessage = "Workspace short name must be defined.")]
        [MaxLength(4, ErrorMessage = "Workspace short name cannot have more than 4 digits.")]
        [MinLength(4, ErrorMessage = "Workspace short name cannot have less than 4 digits.")]
        public string Abbreviation { get; set; }

        [DataMember(Name = "hasValuePerHour", IsRequired = false)]
        public bool HasValuePerHour { get; set; } = false;

        [DataMember(Name = "currencyAbbreviation", IsRequired = false)]
        [MaxLength(3, ErrorMessage = "Currency abbreviation always have 3 digits.")]
        [MinLength(3, ErrorMessage = "Currency abbreviation always have 3 digits.")]
        public string Currency { get; set; }

        [DataMember(Name = "hourlyValues", IsRequired = false)]
        public List<IssueTypeHourlyValue> HourlyValues { get; set; }

    }
}
