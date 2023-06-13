using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Application.Services.SignalR
{
    public interface ISignalRContract
    {
        Task UpdateIssue(string accountId, Guid issueId);
    }
}
