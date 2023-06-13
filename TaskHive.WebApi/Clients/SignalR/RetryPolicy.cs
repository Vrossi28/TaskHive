using Microsoft.AspNetCore.SignalR.Client;

namespace TaskHive.WebApi.Clients.SignalR
{
    public class RetryPolicy : IRetryPolicy
    {
        private readonly Random _random = new Random();
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(_random.NextDouble() * 10);
        }
    }
}
