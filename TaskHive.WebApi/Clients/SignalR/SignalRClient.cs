using Microsoft.AspNetCore.SignalR.Client;
using TaskHive.Application.Services.SignalR;

namespace TaskHive.WebApi.Clients.SignalR
{
    public class SignalRClient : ISignalRContract
    {
        private static HubConnection _connection;
        private ILogger<SignalRClient> _logger;

        public SignalRClient(ILogger<SignalRClient> logger, string url, string accessToken)
        {
            url = url + "?accessToken=" + accessToken;
            _logger = logger;
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect(new RetryPolicy())
                .Build();

            _connection.Closed += async (error) =>
            {
                await Task.Delay(5000);
                await _connection.StartAsync();

                _logger.LogInformation("Reconnecting to SignalR Server in " + url);
            };

            _logger.LogInformation("Connecting to SignalR Server in " + url);

            _connection.StartAsync();
        }

        public async Task UpdateIssue(string accountId, Guid issueId)
        {
            await _connection.InvokeAsync("UpdateIssue", accountId, issueId);
        }
    }
}
