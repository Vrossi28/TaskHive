using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    [DataContract]
    public class RegisterRequest : LoginRequest
    {
        [DataMember(Name = "mobileNumber", IsRequired = false)]
        [MaxLength(15, ErrorMessage = "Mobile numbers are no longer than 15 digits.")]
        [MinLength(4, ErrorMessage = "Mobile numbers have no less than 4 digits.")]
        [Phone]
        public string MobileNumber { get; set; }

        [DataMember(Name = "firstName", IsRequired = true)]
        [MaxLength(15, ErrorMessage = "First name cannot have more than 15 digits.")]
        [MinLength(2, ErrorMessage = "First name cannot have less than 2 digits.")]
        [Required(ErrorMessage = "First name must be defined.")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastName", IsRequired = true)]
        [MaxLength(15, ErrorMessage = "Last name cannot have more than 15 digits.")]
        [MinLength(2, ErrorMessage = "Last name cannot have less than 2 digits.")]
        [Required(ErrorMessage = "Last name must be defined.")]
        public string LastName { get; set; }

        [DataMember(Name = "companyName", IsRequired = true)]
        [MaxLength(30, ErrorMessage = "Company name cannot have more than 30 digits.")]
        [MinLength(2, ErrorMessage = "Company name cannot have less than 2 digits.")]
        [Required(ErrorMessage = "Company name must be defined.")]
        public string CompanyName { get; set; }

        [DataMember(Name = "signUpType", IsRequired = true)]
        [Required(ErrorMessage = "SignUpType must be defined.")]
        public SignUpType SignUpMode { get; set; }

        [DataMember(Name = "culture", IsRequired = false)]
        public string? Culture { get; set; }

        [DataMember(Name = "timeZone", IsRequired = false)]
        public string? TimeZone { get; set; }
    }
}
