using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Application.Contracts.Requests
{
    [DataContract]
    public class LoginRequest
    {
        [DataMember(Name = "email", IsRequired = true)]
        [Required(ErrorMessage = "Email must be defined.")]
        [EmailAddress]
        public string Email { get; set; }

        [DataMember(Name = "password", IsRequired = false)]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
