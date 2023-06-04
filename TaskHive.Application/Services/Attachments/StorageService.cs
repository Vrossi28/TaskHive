using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Net;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.Application.Services.Attachments
{
    public class StorageService : IStorageService
    {
        private AmazonS3Client _s3Client;

        private AmazonS3Client GetClient(AwsCredentials awsCredentials)
        {
            if (_s3Client == null)
            {
                var config = new AmazonS3Config()
                {
                    RegionEndpoint = RegionEndpoint.EUNorth1,
                };
                var credentials = new BasicAWSCredentials(awsCredentials.AccessKey, awsCredentials.AccessKeySecret);
                _s3Client = new AmazonS3Client(credentials, config);
            }
            return _s3Client;
        }

        public async Task<S3ResponseDto> UploadFileAsync(S3UploadObject s3Obj, AwsCredentials awsCredentials)
        {
            var response = new S3ResponseDto();

            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = s3Obj.InputStream,
                    Key = s3Obj.Name,
                    BucketName = s3Obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                var transferUtility = new TransferUtility(GetClient(awsCredentials));

                await transferUtility.UploadAsync(uploadRequest);

                response.StatusCode = 200;
                response.StatusMessage = $"{s3Obj.Name} has been upload successfully";
            }
            catch (AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode;
                response.StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = ex.Message;
            }

            return response;
        }

        public async Task<S3UrlResponse> GetFileUrl(S3UrlObject s3Obj, AwsCredentials awsCredentials)
        {
            var response = new S3UrlResponse();

            try
            {
                var urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = s3Obj.BucketName,
                    Key = s3Obj.Name,
                    Expires = DateTime.UtcNow.AddHours(1),
                    Protocol = Protocol.HTTPS
                };

                var url = GetClient(awsCredentials).GetPreSignedURL(urlRequest);

                response.StatusCode = 200;
                response.StatusMessage = $"Url for {s3Obj.Name} has been retrieved successfully";
                response.FileUrl = url;
                response.ExpiryDate = urlRequest.Expires;
            }
            catch (AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode;
                response.StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = ex.Message;
            }

            return response;
        }

        public async Task<S3ResponseDto> DeleteFileFromBucket(IssueFile issueFile, S3UrlObject s3Object, AwsCredentials awsCredentials)
        {
            var response = new S3ResponseDto();
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = s3Object.BucketName,
                    Key = s3Object.Name
                };

                var responseObj = await GetClient(awsCredentials).DeleteObjectAsync(deleteObjectRequest);

                response.StatusCode = (int)responseObj.HttpStatusCode;
                if (response.StatusCode == (int)HttpStatusCode.NoContent)
                {
                    var issueFileRepository = new IssueFileRepository();

                    var result = await issueFileRepository.DeleteIssueFile(issueFile);
                    response.StatusMessage = result.ToString();
                }
            }
            catch (AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode;
                response.StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = ex.Message;
            }
            return response;
        }

        public async Task<List<IssueFile>> GetIssueFilesFromIssue(Guid issueId, string bucketName, AwsCredentials awsCredentials)
        {
            var issueFileRepository = new IssueFileRepository();

            var issueFiles = issueFileRepository.GetFilesByIssueId(issueId);

            foreach (var issueFile in issueFiles)
            {
                if (issueFile.UrlExpiryDate < DateTime.UtcNow)
                {
                    S3UrlObject s3Obj = new()
                    {
                        BucketName = bucketName,
                        Name = issueFile.FileStoredName
                    };

                    var url = await GetFileUrl(s3Obj, awsCredentials);
                    issueFile.Url = url.FileUrl;
                    issueFile.UrlExpiryDate = url.ExpiryDate;

                    await issueFileRepository.UpdateIssueFile(issueFile);
                }
            }

            return issueFileRepository.GetFilesByIssueId(issueId);
        }
    }
}
