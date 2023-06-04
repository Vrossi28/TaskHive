using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;

namespace TaskHive.Infrastructure.Models
{
    public class InviteDto : Invite
    {
        public string WorkspaceName { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
