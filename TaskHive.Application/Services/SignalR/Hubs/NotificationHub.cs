using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.Application.Services.SignalR.Hubs
{
    public class NotificationHub : Hub, ISignalRContract
    {
        private ILogger<NotificationHub> _logger;
        private IConfiguration _configuration;
        private static Dictionary<string, AccountNotificationInfo> _connections = new();

        public NotificationHub(ILogger<NotificationHub> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override Task OnConnectedAsync()
        {
            string accessToken = (Context?.GetHttpContext()?.Request?.Query["accessToken"]);
            if (!string.IsNullOrEmpty(accessToken) && accessToken == _configuration["SignalRService:AccessToken"])
            {
                _logger.LogInformation("Administrator connected");
                return base.OnConnectedAsync();
            }

            string connectionId = Context.ConnectionId;
            _logger.LogInformation("New hub connection " + connectionId);

            try
            {
                string accountId = (Context?.GetHttpContext()?.Request?.Query["accountId"]);
                if (accountId is null)
                {
                    Context.Abort();
                    _logger.LogError("No account id parameter found, refusing connection " + connectionId);
                    return Task.CompletedTask;
                }

                var account = HandleAccountById(accountId);
                if (account is null)
                {
                    Context.Abort();
                    _logger.LogError("Provided account id was not found, refusing connection " + connectionId);
                    return Task.CompletedTask;
                }

                lock (_connections)
                {
                    if (_connections.ContainsKey(accountId))
                    {
                        _connections[accountId].ConnectionId = connectionId;
                    }
                    else
                    {
                        _connections.Add(accountId, new AccountNotificationInfo()
                        {
                            ConnectionId = connectionId,
                            CompanyId = account.CompanyId
                        });
                        JoinGroup(accountId, account.CompanyId.ToString().ToUpper()).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                Context.Abort();
                _logger.LogError("An error occurred in the connection " + connectionId, ex);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            _logger.LogInformation($"New hub disconnection {connectionId} {(exception != null ? " - " + exception.Message : "")}");

            lock (_connections)
            {
                var userId = (from l in _connections where l.Value.ConnectionId == connectionId select l).FirstOrDefault();

                if (!String.IsNullOrEmpty(userId.Key))
                {
                    if (_connections.ContainsKey(userId.Key))
                    {
                        Task.Run(async () => await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.Value.CompanyId.ToString().ToUpper()));
                        _connections.Remove(userId.Key);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGroup(string accountId, string companyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, companyId);
            await Clients.Group(companyId).SendAsync("ReceiveMessage", Context.ConnectionId, $"{accountId} joined group {companyId}");
        }

        public async Task UpdateIssue(string accountId, Guid issueId)
        {
            var account = HandleAccountById(accountId);
            if (account != null)
            {
                await Clients.Group(account.CompanyId.ToString().ToUpper()).SendAsync("OnIssueUpdate", issueId);
                _logger.LogInformation("Issue update event fired - " + issueId);
            }
        }

        private Account HandleAccountById(string accountId)
        {
            Account acc = null;
            var isGuid = Guid.TryParse(accountId, out Guid guidId);
            if (!isGuid)
            {
                Context.Abort();
                
                return acc;
            }

            AccountRepository accountRepository = new();
            acc = accountRepository.GetAccountById(guidId).Result;
            return acc;
        }
    }
}
