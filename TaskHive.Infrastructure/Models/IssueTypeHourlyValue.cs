using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;
using TaskHive.Entities.Enums;

namespace TaskHive.Infrastructure.Models
{
    public class IssueTypeHourlyValue
    {
        [DataMember(Name = "issueTypeId", IsRequired = true)]
        [Range(1, 23, ErrorMessage = "Issue type identification must be between 1 and 23.")]
        public IssueType IssueTypeId { get; set; }

        [DataMember(Name = "valuePerHour", IsRequired = true)]
        public decimal ValuePerHour { get; set; }
    }
}
