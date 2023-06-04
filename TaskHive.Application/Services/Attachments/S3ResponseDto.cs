using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Application.Services.Attachments
{
    public class S3ResponseDto
    {
        public int StatusCode { get; set; } = 200;
        public string StatusMessage { get; set; } = string.Empty;
    }
}
