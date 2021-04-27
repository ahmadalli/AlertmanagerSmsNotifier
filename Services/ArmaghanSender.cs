using AlertmanagerSmsNotifier.Models.Configs;
using AlertmanagerSmsNotifier.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AlertmanagerSmsNotifier.Services
{
    public class ArmaghanSender : ISmsSender
    {
        private readonly HttpClient _httpClient;
        private readonly IOptionsMonitor<ArmaghanConfigs> _configsMonitor;
        private readonly ILogger<ArmaghanSender> _logger;

        public ArmaghanSender(HttpClient httpClient, IOptionsMonitor<ArmaghanConfigs> configsMonitor, ILogger<ArmaghanSender> logger)
        {
            _httpClient = httpClient;
            _configsMonitor = configsMonitor;
            _logger = logger;
        }

        public async Task SendSms(string text, string[] recipients)
        {
            var configs = _configsMonitor.CurrentValue;


            foreach (var recipient in recipients)
            {
                if (string.IsNullOrWhiteSpace(recipient) || !Regex.IsMatch(recipient, @"^\d+$") || recipient.Length != 12)
                {
                    _logger.LogError($"invalid recipient {recipient}");
                    continue;
                }

                var url = $"http://negar.armaghan.net/sms/url_send.html?" +
                    $"originator={configs.Originator}" +
                    $"&destination={recipient}" +
                    $"&content={HttpUtility.UrlEncode(text)}" +
                    $"&username={HttpUtility.UrlEncode(configs.Username)}" +
                    $"&password={HttpUtility.UrlEncode(configs.Password)}";

                var result = await _httpClient.GetAsync(url);

                var content = await result.Content.ReadAsStringAsync();
                if (!result.IsSuccessStatusCode || content.Contains("error", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogError("armaghan sms failed" +
                        $"{Environment.NewLine}Status Code: {result.StatusCode}" +
                        $"{Environment.NewLine}Message:{Environment.NewLine}{content.Trim()}");
                }
            }
        }
    }
}
