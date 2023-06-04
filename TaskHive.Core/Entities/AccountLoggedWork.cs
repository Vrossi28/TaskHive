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
    public class AccountLoggedWork
    {
        [Key]
        public Guid AccountLoggedWorkId { get; set; }
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public Guid IssueId { get; set; }
        [Required]
        public decimal TimeSpent { get; set; }
        [Required]
        public DateTime StartingDate { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }
    }
}
