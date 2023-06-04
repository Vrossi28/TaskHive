using System.Net.Mail;
using System.Net;
using Humanizer;
using Microsoft.Extensions.Configuration;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Repositories;
using TaskHive.Infrastructure.Models;

namespace TaskHive.Application.Services.Email
{
    public class EmailService
    {
        private static readonly string ROOT_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "//Services//Email//HtmlTemplates//");
        private static string SENDER_EMAIL= "";
        private static string SENDER_PASSWORD = "";
        private static string WEB_APP_URL = "";
        public EmailService()
        {
            var configuration = new ConfigurationBuilder();

            configuration.SetBasePath(System.IO.Directory.GetCurrentDirectory());
            configuration.AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: false);
            configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true);
            configuration.AddEnvironmentVariables();

            IConfigurationRoot Configuration = configuration.Build();

            SENDER_EMAIL = Configuration[EmailConstants.SenderEmail];
            SENDER_PASSWORD = Configuration[EmailConstants.SenderPassword];
            WEB_APP_URL = Configuration[EmailConstants.WebAppUrl];
        }

        private bool SendEmail(string subject, string email, string body, MailPriority priority)
        {
            try
            {
                MailMessage mailMessage = new()
                {
                    From = new MailAddress(SENDER_EMAIL, "Task Hive")
                };
                mailMessage.To.Add(new MailAddress(email));
                mailMessage.Subject = subject;
                mailMessage.Priority = priority;

                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;

                SmtpClient smtpClient = new()
                {
                    EnableSsl = true,
                    Host = "smtp.office365.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SENDER_EMAIL, SENDER_PASSWORD),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                smtpClient.Send(mailMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> SendInvitationEmail(Invite invite, Account inviter, string workspaceName)
        {
            string subject = $"{inviter.FirstName} {inviter.LastName} invited you to join {workspaceName} in Task Hive";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "Invite.html");

            var body = htmlFile.Replace("@INVITER_NAME", $"{inviter.FirstName} {inviter.LastName}")
                .Replace("@REDIRECT_LINK", $"{WEB_APP_URL}/register/{invite.InviteId}");

            return SendEmail(subject, invite.InvitedEmail, body, MailPriority.Normal);
        }

        public async Task<bool> SendVerificationEmail(Account account)
        {
            string subject = $"Email confirmation";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "Verify.html");

            var body = htmlFile.Replace("@ACCOUNT_NAME", $"{account.FirstName} {account.LastName}")
                .Replace("@REDIRECT_LINK", $"{WEB_APP_URL}/account/verify/{account.VerificationToken}");

            return SendEmail(subject, account.Email, body, MailPriority.Normal);
        }

        public async Task<bool> SendTaskAssignedEmail(Account account, Issue issue, string assigner)
        {
            string subject = $"{issue.Title} has been assigned to you";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "NewTaskAssignment.html");

            var body = htmlFile.Replace("@USER_NAME", $"{account.FirstName} {account.LastName}")
                .Replace("@TASK_NAME", $"{issue.Title}")
                .Replace("@REDIRECT_LINK", $"{WEB_APP_URL}/tasks/{issue.IssueId}");

            return SendEmail(subject, account.Email, body, MailPriority.Normal);
        }

        public async Task SendTaskClosedEmail(Issue issue, IssueResolution resolutionDetail)
        {
            AccountRepository accountRepository = new();
            var resolver = await accountRepository.GetAccountById(resolutionDetail.SolverAccountId);
            var creator = await accountRepository.GetAccountById(issue.CreatedBy);
            var assignee = await accountRepository.GetAccountById(issue.CurrentAssignee);

            List<Account> targets = new();
            targets.Add(resolver);
            if (assignee.AccountId != resolver.AccountId && assignee.AccountId != creator.AccountId)
            {
                targets.Add(assignee);
            }
            else if (creator.AccountId != resolver.AccountId && creator.AccountId != assignee.AccountId)
            {
                targets.Add(creator);
            }

            string subject = $"{issue.Title} has been closed with status {resolutionDetail.Type.Humanize()}";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "TaskResolved.html");

            foreach (var target in targets)
            {
                var resolverName = resolver.FirstName + " " + resolver.LastName;
                if (resolver.AccountId == target.AccountId)
                {
                    resolverName = "you";
                }

                var body = htmlFile.Replace("@USER_NAME", $"{target.FirstName} {target.LastName}")
                    .Replace("@TASK_NAME", issue.Title)
                    .Replace("@RESOLUTION_STATUS", resolutionDetail.Type.Humanize())
                    .Replace("@RESOLVER_NAME", resolverName)
                    .Replace("@RESOLUTION_DESCRIPTION", resolutionDetail.Description)
                    .Replace("@REDIRECT_LINK", $"{WEB_APP_URL}/tasks/{issue.IssueId}");
                SendEmail(subject, target.Email, body, MailPriority.Normal);
            }
        }

        public async Task<bool> SendNewWorkspaceEmail(Account account, Workspace workspace, string assigner)
        {
            string subject = $"You has been added to {workspace.Name} workspace";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "NewWorkspace.html");

            var body = htmlFile.Replace("@USER_NAME", $"{account.FirstName} {account.LastName}")
                .Replace("@WORKSPACE_NAME", workspace.Name)
                .Replace("@REDIRECT_LINK", $"{WEB_APP_URL}/workspaces/{workspace.WorkspaceId}");

            return SendEmail(subject, account.Email, body, MailPriority.Normal);
        }

        public async Task<bool> SendResetPasswordEmail(Account account)
        {
            string subject = $"Reset password";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "ResetPassword.html");

            var body = htmlFile.Replace("@ACCOUNT_NAME", $"{account.FirstName} {account.LastName}")
                .Replace("@REDIRECT_LINK", $"{WEB_APP_URL}/reset-password/{account.PasswordResetToken}");

            return SendEmail(subject, account.Email, body, MailPriority.Normal);
        }

        public async Task<bool> SendSuspiciousLoginEmail(Account account, GeolocationDetails geolocation)
        {
            string subject = $"SUSPICIOUS LOGIN ACTIVITY ON YOUR ACCOUNT";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "SuspiciousLogin.html");

            var body = htmlFile.Replace("@ACCOUNT_NAME", $"{account.FirstName} {account.LastName}")
                .Replace("@REDIRECT_LINK", $"{WEB_APP_URL}/reset-password/{account.PasswordResetToken}")
                .Replace("@IP_ADDRESS", $"{geolocation.Query}")
                .Replace("@CITY_COUNTRY", $"{geolocation.City}, {geolocation.Country}");

            return SendEmail(subject, account.Email, body, MailPriority.Normal);
        }

        public async Task<bool> SendFeedbackEmail(string feedbacker, string subject, string message)
        {
            string subjectEmail = $"{feedbacker} is getting in touch";
            var htmlFile = await File.ReadAllTextAsync(ROOT_PATH + "Feedback.html");

            var body = htmlFile
                .Replace("@SENDER", $"{feedbacker}")
                .Replace("@SUBJECT", $"{subject}")
                .Replace("@MESSAGE", $"{message}");

            return SendEmail(subjectEmail, "taskhive@outlook.com", body, MailPriority.Normal);
        }
    }
}