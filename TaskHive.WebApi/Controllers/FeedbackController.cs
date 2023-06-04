using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TaskHive.Application.Contracts.Requests;
using TaskHive.Application.Services.Email;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        /// <summary>
        /// Sends an email to system administrator informing feedback details
        /// </summary>
        /// <response code="200">Email will be sent</response>
        [HttpPost("feedback")]
        public async Task<IActionResult> SendFeedback(FeedbackRequest feedbackRequest)
        {
            EmailService emailService = new();
            BackgroundJob.Enqueue(() => emailService.SendFeedbackEmail(feedbackRequest.FeedbackerEmail, feedbackRequest.Subject, feedbackRequest.Message));

            return Ok(new { message = "Thanks for you message! We will get back to you shortly." });
        }
    }
}
