using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TaskHive.Infrastructure.Models;

namespace TaskHive.Application.Contracts.Requests
{
    [DataContract]
    public class AddAccountWorkspaceRequest
    {
        [DataMember(Name = "workspaceId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace id must be defined.")]
        public Guid WorkspaceId { get; set; }

        [DataMember(Name = "accountIds", IsRequired = true)]
        [Required(ErrorMessage = "Account identifications must be defined.")]
        public List<AccountToAdd> AccountsToAdd { get; set; }
    }
}
