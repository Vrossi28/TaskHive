using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskHive.Core.Entities
{
    public class IssueFile
    {
        [Key]
        public Guid IssueFileId { get; set; }

        [Required]
        public Guid IssueId { get; set; }

        [Required]
        [MaxLength(256)]
        public string FileFriendlyName { get; set; }

        [Required]
        [MaxLength(256)]
        public string FileStoredName { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public DateTime UrlExpiryDate { get; set; }

        [JsonIgnore]
        public virtual Issue Issue { get; set; }
    }
}
