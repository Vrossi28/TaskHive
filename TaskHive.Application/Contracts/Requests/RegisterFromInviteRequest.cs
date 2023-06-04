using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TaskHive.Application.Contracts.Requests
{
    public class RegisterFromInviteRequest
    {
        [DataMember(Name = "password", IsRequired = true)]
        [PasswordPropertyText]
        [MinLength(6, ErrorMessage = "Password cannot have less than 2 digits.")]
        [Required(ErrorMessage = "Password must be defined.")]
        public string Password { get; set; }

        [DataMember(Name = "mobileNumber", IsRequired = false)]
        [MaxLength(15, ErrorMessage = "Mobile numbers are no longer than 15 digits.")]
        [MinLength(4, ErrorMessage = "Mobile numbers have no less than 4 digits.")]
        [Phone]
        public string MobileNumber { get; set; }

        [DataMember(Name = "firstName", IsRequired = true)]
        [MaxLength(15, ErrorMessage = "First name cannot have more than 15 digits.")]
        [MinLength(4, ErrorMessage = "First name cannot have less than 2 digits.")]
        [Required(ErrorMessage = "First name must be defined.")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastName", IsRequired = true)]
        [MaxLength(15, ErrorMessage = "Last name cannot have more than 15 digits.")]
        [MinLength(4, ErrorMessage = "Last name cannot have less than 2 digits.")]
        [Required(ErrorMessage = "Last name must be defined.")]
        public string LastName { get; set; }

        [DataMember(Name = "culture", IsRequired = false)]
        public string? Culture { get; set; }

        [DataMember(Name = "timeZone", IsRequired = false)]
        public string? TimeZone { get; set; }
    }
}
