using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Application.Services.SignalR
{
    public class AccountNotificationInfo
    {
        public Guid CompanyId { get; set; }
        public string ConnectionId { get; set; }
    }
}
