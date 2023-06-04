using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    [DataContract]
    public class CreateInviteRequest
    {
        [DataMember(Name = "workspaceId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace identification must be defined.")]
        public Guid WorkspaceId { get; set; }

        [DataMember(Name = "roleId", IsRequired = true)]
        [Required(ErrorMessage = "RoleId must be defined.")]
        [Range(1, 5, ErrorMessage = "Role identification must be between 1 and 5.")]
        public RoleType RoleId { get; set; }

        [DataMember(Name = "invitedEmail", IsRequired = false)]
        [Required(ErrorMessage = "Invited email must be defined.")]
        [EmailAddress]
        public string InvitedEmail { get; set; }
    }
}
