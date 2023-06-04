using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using TaskHive.Entities.Enums;
using Hangfire;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Repositories;
using TaskHive.Application.Services.Email;
using Microsoft.Extensions.Logging;

namespace TaskHive.Application.Services.Security
{

    public class AccessNotifierHandler
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _settings;
        private readonly ILogger _logger;

        public AccessNotifierHandler(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration[SecurityConstants.GeolocationUrl])
            };

            _settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = { new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() } }
            };
        }

        public async Task<GeolocationDetails> AccessLocationDetailsHandler(IPAddress ipAddress, Account account, string resetPasswordToken)
        {
            GeolocationDetails geolocation = await GetGeolocationDetails(ipAddress);

            if (geolocation == null || geolocation.Status == GeolocationStatus.Fail)
            {
                _logger.LogWarning($"Account {account.AccountId} | Not possible to get geolocation");
                geolocation.City = string.Empty;
                geolocation.Country = string.Empty;
            }
            else
            {
                AccountAccessLocationRepository accessLocationRepository = new();
                AccountRepository accountRepository = new();

                var access = accessLocationRepository.GetAccessLocationByAccount(account.AccountId);
                var firstAccess = access.Count == 0;
                var hasOriginCountry = account.OriginCountry?.Length > 2;
                if (!hasOriginCountry && firstAccess)
                {
                    account.OriginCountry = geolocation.Country;
                    await accountRepository.UpdateAccount(account);
                    return geolocation;
                }

                var isRecurrent = await CheckLocationRecurrence(geolocation.Country, geolocation.City, access);

                if (hasOriginCountry && account.OriginCountry != geolocation.Country && !isRecurrent)
                {
                    account.PasswordResetToken = resetPasswordToken;
                    account.ResetTokenExpiration = DateTime.UtcNow.AddMinutes(15);

                    var accountUpdated = await accountRepository.UpdateAccount(account);
                    if (accountUpdated)
                    {
                        EmailService emailService = new();
                        BackgroundJob.Enqueue(() => emailService.SendSuspiciousLoginEmail(account, geolocation));
                    }
                }

            }
            return geolocation;

        }

        public async Task<GeolocationDetails> GetGeolocationDetails(IPAddress ipAddress)
        {
            GeolocationDetails geolocation = null;
            using (_httpClient)
            {
                var response = await _httpClient.GetAsync($"{ipAddress}");
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    geolocation = JsonConvert.DeserializeObject<GeolocationDetails>(json, _settings);
            }
            return geolocation;
        }

        private async Task<bool> CheckLocationRecurrence(string country, string city, List<AccountAccessLocation> access)
        {
            var count = access.Count(a => a.Country == country && a.City == city);

            bool isRecurrent = count > 5;

            return isRecurrent;
        }
    }
}
