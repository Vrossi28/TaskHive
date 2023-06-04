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
    public class AccountWorkspace
    {
        [Key]
        public Guid AccountWorkspaceId { get; set; }
        [Required]
        public Guid WorkspaceId { get; set; }
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public RoleType RoleId { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }
        [JsonIgnore]
        public virtual Workspace Workspace { get; set; }
        [JsonIgnore]
        public virtual AccountRoleDesc RoleDesc { get; set; }
    }
}
