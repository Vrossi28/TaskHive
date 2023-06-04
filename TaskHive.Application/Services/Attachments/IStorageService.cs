using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;

namespace TaskHive.Application.Services.Attachments
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3UploadObject s3Obj, AwsCredentials awsCredentials);
        Task<S3UrlResponse> GetFileUrl(S3UrlObject s3Obj, AwsCredentials awsCredentials);
        Task<List<IssueFile>> GetIssueFilesFromIssue(Guid issueId, string bucketName, AwsCredentials awsCredentials);
        Task<S3ResponseDto> DeleteFileFromBucket(IssueFile issueFile, S3UrlObject s3Object, AwsCredentials awsCredentials);
    }
}
