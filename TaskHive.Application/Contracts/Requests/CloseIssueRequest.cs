using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    [DataContract]
    public class CloseIssueRequest
    {
        [DataMember(Name = "resolutionId", IsRequired = true)]
        [Required(ErrorMessage = "ResolutionId must be defined.")]
        [Range(1, 15, ErrorMessage = "Resolution identification must be between 1 and 15.")]
        public ResolutionType IssueResolutionId { get; set; }

        [DataMember(Name = "description", IsRequired = false)]
        public string Description { get; set; }
    }
}
