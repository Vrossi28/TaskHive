using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;
using System;

namespace TaskHive.Application.Contracts.Requests
{
    public class LogWorkRequest
    {
        [DataMember(Name = "issueId", IsRequired = true)]
        [Required(ErrorMessage = "Issue identification must be defined.")]
        public Guid IssueId { get; set; }

        [DataMember(Name = "timeSpent", IsRequired = true)]
        [Required(ErrorMessage = "Time spent must be defined.")]
        public decimal TimeSpent { get; set; }

        [DataMember(Name = "startingDate", IsRequired = true)]
        [Required(ErrorMessage = "Starting date must be defined.")]
        public DateTime StartingDate { get; set; }

        [DataMember(Name = "description", IsRequired = false)]
        public string? Description { get; set; }
    }
}
