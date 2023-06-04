using System.ComponentModel.DataAnnotations;
using System;
using System.Runtime.Serialization;
using TaskHive.Entities.Enums;
using System.Collections.Generic;
using System.Linq;
using TaskHive.Core.Enums;

namespace TaskHive.Application.Contracts.Requests
{
    [DataContract]
    public class AddIssueRequest
    {
        [DataMember(Name = "title", IsRequired = true)]
        [Required(ErrorMessage = "Issue title must be defined.")]
        public string Title { get; set; }

        [DataMember(Name = "title", IsRequired = false)]
        public string Description { get; set; }

        [DataMember(Name = "workspaceId", IsRequired = true)]
        [Required(ErrorMessage = "Workspace identification must be defined.")]
        public Guid WorkspaceId { get; set; }

        [DataMember(Name = "assignedToAccountId", IsRequired = true)]
        [Required(ErrorMessage = "Account identification must be defined.")]
        public Guid AssignedToAccId { get; set; }

        [DataMember(Name = "parentId", IsRequired = false)]
        public Guid? ParentId { get; set; }

        [DataMember(Name = "issueTypeId", IsRequired = true)]
        [Required(ErrorMessage = "Issue type must be defined.")]
        [Range(1, 23, ErrorMessage = "Issue type identification must be between 1 and 23.")]
        public IssueType IssueTypeId { get; set; }

        [DataMember(Name = "priorityId", IsRequired = true)]
        [Required(ErrorMessage = "Priority must be defined.")]
        [Range(1, 4, ErrorMessage = "Priority identification must be between 1 and 4.")]
        public PriorityType PriorityId { get; set; }

        [DataMember(Name = "estimatedTime", IsRequired = false)]
        public decimal? EstimatedTime { get; set; }
    }
}
