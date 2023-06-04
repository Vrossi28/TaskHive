using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;
using Newtonsoft.Json;

namespace TaskHive.Core.Entities
{
    public class Invite
    {
        [Key]
        public Guid InviteId { get; set; }
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public RoleType RoleId { get; set; }
        [Required]
        [MaxLength(256)]
        public string InvitedEmail { get; set; }
        [Required]
        public Guid WorkspaceId { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }
    }
}
