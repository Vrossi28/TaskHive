using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;

namespace TaskHive.Core.Entities
{
    public class Account
    {
        [Key]
        public Guid AccountId { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }
        [Required]
        public string HashedPassword { get; set; }
        public string MobileNumber { get; set; }
        [Required]
        public State AccountState { get; set; } = State.Active;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public RoleType RoleId { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiration { get; set; }
        public string? OriginCountry { get; set; }
        [Required]
        public SignUpType SignUpType { get; set; }
        public string Culture { get; set; } = "en-US";
        public bool Premium { get; set; } = false;
        public DateTime? PremiumExpirationDate
        {
            get { return _premiumExpirationDate; }
            set
            {
                _premiumExpirationDate = value;
                if (value.HasValue)
                {
                    RenewalReminderDate = value.Value.AddDays(-7);
                }
            }
        }
        private DateTime? _premiumExpirationDate;
        public DateTime? RenewalReminderDate { get; set; }
        public string TimeZone { get; set; } = "00:00";

        public virtual ICollection<AccountLoggedWork> AccountLoggedWorks { get; set; }
        public virtual ICollection<AccountWorkspace> AccountWorkspaces { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }
        public virtual ICollection<AccountAccessLocation> AccountAccessLocations { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }  
        public virtual ICollection<IssueComment> IssueComments { get; set; }
        public virtual AccountRoleDesc Role { get; set; }
        public virtual Company Company { get; set; }
    }
}
