using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TaskHive.Infrastructure.Models;

namespace TaskHive.Application.Contracts.Requests
{
    [DataContract]
    public class GetAccountsByEmailRequest
    {
        [DataMember(Name = "emailAddresses", IsRequired = true)]
        [Required(ErrorMessage = "At least one email address must be defined.")]
        public List<EmailAccounts> EmailAddresses { get; set; }
    }
}
