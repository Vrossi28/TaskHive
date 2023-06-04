using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Enums;

namespace TaskHive.Infrastructure.Models
{
    public class AccountToAdd
    {
        [DataMember(Name = "accountId", IsRequired = true)]
        [Required(ErrorMessage = "Account identification must be defined.")]
        public Guid AccountId { get; set; }

        [DataMember(Name = "roleId", IsRequired = true)]
        [Required(ErrorMessage = "RoleId must be defined.")]
        [Range(1, 5, ErrorMessage = "Role identification must be between 1 and 5.")]
        public RoleType RoleId { get; set; }
    }
}
