using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TaskHive.Application.Contracts.Requests
{
    public class ResetPasswordRequest
    {
        [DataMember(Name = "token", IsRequired = true)]
        [Required(ErrorMessage = "Reset password token must be defined.")]
        public string Token { get; set; }

        [DataMember(Name = "password", IsRequired = true)]
        [PasswordPropertyText]
        [MinLength(6, ErrorMessage = "Password cannot have less than 2 digits.")]
        [Required(ErrorMessage = "Password must be defined.")]
        public string Password { get; set; }
    }
}
