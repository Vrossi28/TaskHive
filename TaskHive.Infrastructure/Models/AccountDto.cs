using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Infrastructure.Models
{
    public class AccountDto
    {
        public Guid AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? OriginCountry { get; set; }
        public bool Premium { get; set; }
        public DateTime? PremiumExpirationDate { get; set; }
        public string Culture { get; set; }
        public string TimeZone { get; set; }

    }
}
