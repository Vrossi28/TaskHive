using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Linq;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    public class AddCommentRequest
    {
        [DataMember(Name = "accountId", IsRequired = true)]
        [Required(ErrorMessage = "Account identification must be defined.")]
        public Guid AccountId { get; set; }

        [DataMember(Name = "comment", IsRequired = true)]
        [MinLength(3)]
        public string Comment { get; set; }
    }
}
