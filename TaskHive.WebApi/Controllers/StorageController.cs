using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using TaskHive.Application.Services.Attachments;
using TaskHive.Application.Services.SignalR;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class StorageController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly ISignalRContract _signalRContract;

        public StorageController(IStorageService storageService, IConfiguration configuration, ISignalRContract signalRContract)
        {
            _storageService = storageService;
            _configuration = configuration;
            _signalRContract = signalRContract;
        }

        /// <summary>
        /// Stores a file
        /// </summary>
        /// <response code="200">File saved</response>
        /// <response code="400">Empty file</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="409">Not possible to save file at AWS</response>
        /// <response code="422">File size cannot be higher than 10MB</response>
        /// <response code="500">Internal error</response>
        [HttpPost("storage/issue/{issueId}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file, Guid issueId)
        {
            IssueRepository issueRepository = new();
            var existing = await issueRepository.GetIssueByIdAsync(issueId);
            if (existing == null) return NotFound("Issue not found.");

            AccountRepository accountRepository = new();
            var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
            var user = await accountRepository.GetActiveAccountByEmailAsync(email);
            if (user == null) return NotFound(new { message = "User not found." });

            if (file == null || file.Length == 0) return BadRequest("Empty file.");

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png"
                && file.ContentType != "application/pdf" && file.ContentType != "text/csv"
                && file.ContentType != "application/msword" && file.ContentType != "application/vnd.ms-powerpoint"
                && file.ContentType != "application/vnd.ms-excel" && file.ContentType != "application/zip"
                && file.ContentType != "application/xml" && file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                && file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                && file.ContentType != "text/plain" && file.ContentType != "text/xml")
                return Conflict("Not allowed file type.");

            if (file.Length > 10485760) return UnprocessableEntity("File size cannot be higher than 10MB.");

            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            Guid fileId = Guid.NewGuid();

            var fileName = fileId.ToString() + Path.GetExtension(file.FileName);

            var s3Obj = new S3UploadObject()
            {
                BucketName = _configuration[AwsConstants.BucketName],
                InputStream = memoryStream,
                Name = fileName
            };

            var credentials = new AwsCredentials()
            {
                AccessKey = _configuration[AwsConstants.AccessKey],
                AccessKeySecret = _configuration[AwsConstants.SecretKey]
            };

            var result = await _storageService.UploadFileAsync(s3Obj, credentials);

            if (result.StatusCode != (int)HttpStatusCode.OK)
                return Conflict(result.StatusMessage);

            var s3UrlObj = new S3UrlObject()
            {
                BucketName = s3Obj.BucketName,
                Name = s3Obj.Name
            };

            var url = await _storageService.GetFileUrl(s3UrlObj, credentials);
            if (url.StatusCode != (int)HttpStatusCode.OK)
                return Conflict(url.StatusMessage);

            IssueFileRepository issueFileRepository = new();
            IssueFile issueFile = new()
            {
                IssueFileId = fileId,
                FileFriendlyName = file.FileName,
                FileStoredName = fileName,
                IssueId = issueId,
                Url = url.FileUrl,
                UrlExpiryDate = url.ExpiryDate
            };

            bool added = await issueFileRepository.AddFile(issueFile);
            if (!added)
                return Problem();

            await _signalRContract.UpdateIssue(user.AccountId.ToString(), issueId);
            return Ok(result);
        }

        /// <summary>
        /// Provides a list of files from issue
        /// </summary>
        /// <response code="200">List of files</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("storage/issue/{issueId}/files")]
        [Authorize]
        public async Task<IActionResult> GetFilesFromIssue(Guid issueId)
        {
            AccountRepository accountRepository = new();
            var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
            var user = await accountRepository.GetActiveAccountByEmailAsync(email);
            if (user == null) return NotFound(new { message = "User not found." });

            IssueRepository issueRepository = new();
            var existing = await issueRepository.GetIssueByIdAsync(issueId);
            if (existing == null) return NotFound("Issue not found.");

            var credentials = new AwsCredentials()
            {
                AccessKey = _configuration[AwsConstants.AccessKey],
                AccessKeySecret = _configuration[AwsConstants.SecretKey]
            };

            var bucketName = _configuration[AwsConstants.BucketName];

            var files = await _storageService.GetIssueFilesFromIssue(issueId, bucketName, credentials);
            var json = JsonConvert.SerializeObject(files, Formatting.Indented);

            return Ok(json);
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <response code="204">File deleted</response>
        /// <response code="400">Invalid authentication</response>
        /// <response code="404">File not found</response>
        /// <response code="409">Not possible to delete file at AWS</response>
        /// <response code="500">Internal error</response>
        [HttpDelete("storage/issue/{issueId}/files/{issueFileId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFileFromIssue(Guid issueId, Guid issueFileId)
        {
            AccountRepository accountRepository = new();
            var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
            var user = await accountRepository.GetActiveAccountByEmailAsync(email);
            if (user == null) return BadRequest(new { message = "User not found." });

            var credentials = new AwsCredentials()
            {
                AccessKey = _configuration[AwsConstants.AccessKey],
                AccessKeySecret = _configuration[AwsConstants.SecretKey]
            };

            var bucketName = _configuration[AwsConstants.BucketName];

            var issueFileRepository = new IssueFileRepository();

            var existing = await issueFileRepository.GetFileByIds(issueId, issueFileId);

            if (existing != null)
            {
                S3UrlObject s3Obj = new()
                {
                    BucketName = bucketName,
                    Name = existing.FileStoredName
                };

                var files = await _storageService.DeleteFileFromBucket(existing, s3Obj, credentials);
                var json = JsonConvert.SerializeObject(files, Formatting.Indented);
                if ((HttpStatusCode)files.StatusCode != HttpStatusCode.NoContent)
                {
                    return Conflict(json);
                }

                await _signalRContract.UpdateIssue(user.AccountId.ToString(), issueId);
                return NoContent();
            }

            return NotFound();
        }
    }
}
