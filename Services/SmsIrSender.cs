using AlertmanagerSmsNotifier.Models.Configs;
using AlertmanagerSmsNotifier.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmsIrRestful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerSmsNotifier.Services
{
    public class SmsIrSender : ISmsSender
    {
        private readonly IOptionsMonitor<SmsIrConfigs> _configsMonitor;
        private readonly ILogger<SmsIrSender> _logger;

        public SmsIrSender(IOptionsMonitor<SmsIrConfigs> configsMonitor, ILogger<SmsIrSender> logger)
        {
            _configsMonitor = configsMonitor;
            _logger = logger;
        }

        public async Task SendSms(string text, string[] recipients)
        {
            var configs = _configsMonitor.CurrentValue;

            var smsPayload = new MessageSendObject()
            {
                Messages = new[] { text },
                MobileNumbers = recipients,
                LineNumber = configs.LineNumber,
                CanContinueInCaseOfError = true
            };

            var token = await getToken();

            var tcs = new TaskCompletionSource<MessageSendResponseObject>();
            await Task.Run(() =>
            {
                tcs.SetResult(new MessageSend().Send(token, smsPayload));
            });

            var response = await tcs.Task;

            if (!response.IsSuccessful)
            {
                _logger.LogError($"sending sms failed.{Environment.NewLine}provider response:{Environment.NewLine}{response.Message}");
            }
        }

        private async Task<string> getToken()
        {
            var tcs = new TaskCompletionSource<string>();

            await Task.Run(() =>
            {
                var configs = _configsMonitor.CurrentValue;

                var tk = new Token();
                string result = tk.GetToken(configs.UserApiKey, configs.SecretKey);

                tcs.SetResult(result);
            });

            return await tcs.Task;
        }
    }
}
