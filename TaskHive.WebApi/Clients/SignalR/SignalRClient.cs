using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.SignalR.Client;
using TaskHive.Application.Services.SignalR;

namespace TaskHive.WebApi.Clients.SignalR
{
    public class SignalRClient : ISignalRContract
    {
        private static HubConnection _connection;
        private ILogger<SignalRClient> _logger;
        private static string _url;

        public SignalRClient(ILogger<SignalRClient> logger, string url, string accessToken)
        {
            _url = url + "?accessToken=" + accessToken;
            _logger = logger;
            _connection = new HubConnectionBuilder()
                .WithUrl(_url, options =>
                {
                    options.HttpMessageHandlerFactory = handler =>
                    {
                        if (handler is HttpClientHandler clientHandler)
                        {
                            clientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        }
                        return handler;
                    };
                })
                .WithAutomaticReconnect(new RetryPolicy())
                .Build();

            _connection.Closed += async (error) =>
            {
                await Task.Delay(5000);
                await _connection.StartAsync();

                _logger.LogInformation("Reconnecting to SignalR Server in " + _url);
            };

            try
            {
                _logger.LogInformation("Connecting to SignalR Server in " + _url);
                _connection.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.StackTrace);
                _logger.LogError(ex.InnerException.Message);
            }

            _logger.LogInformation("Connection state: " + _connection.State);
        }

        public async Task UpdateIssue(string accountId, Guid issueId)
        {
            await _connection.InvokeAsync("UpdateIssue", accountId, issueId);
        }
    }
}
