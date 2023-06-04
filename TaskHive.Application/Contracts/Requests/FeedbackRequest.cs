using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TaskHive.Application.Contracts.Requests
{
    public class FeedbackRequest
    {
        [DataMember(Name = "feedbackerEmail", IsRequired = true)]
        [Required(ErrorMessage = "Feedbacker email must be defined.")]
        public string FeedbackerEmail { get; set; }

        [DataMember(Name = "subject", IsRequired = true)]
        [Required(ErrorMessage = "Subject must be defined.")]
        public string Subject { get; set; }

        [DataMember(Name = "message", IsRequired = true)]
        [Required(ErrorMessage = "Message must be defined.")]
        public string Message { get; set; }
    }
}
