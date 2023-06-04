using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Core.Entities
{
    public class AccountAccessLocation
    {
        [Key]
        public Guid AccountAccessLocationId { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public DateTime AccessDate { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public virtual Account Account { get; set; }
    }
}
