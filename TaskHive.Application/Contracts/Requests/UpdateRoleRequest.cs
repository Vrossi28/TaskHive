using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    public class UpdateRoleRequest
    {
        [DataMember(Name = "workspaceId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace id must be defined.")]
        public Guid WorkspaceId { get; set; }

        [DataMember(Name = "accountId", IsRequired = true)]
        [Required(ErrorMessage = "Account id must be defined.")]
        public Guid AccountId { get; set; }

        [DataMember(Name = "roleId", IsRequired = true)]
        [Required(ErrorMessage = "RoleId must be defined.")]
        [Range(1, 5, ErrorMessage = "Role identification must be between 1 and 5.")]
        public RoleType RoleId { get; set; }
    }
}
