using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Application.Services.Attachments
{
    public class S3UrlResponse : S3ResponseDto
    {
        public string FileUrl { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
