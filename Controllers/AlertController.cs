using AlertmanagerSmsNotifier.Models;
using AlertmanagerSmsNotifier.Models.Configs;
using AlertmanagerSmsNotifier.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerSmsNotifier.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly ISmsSender _smsSender;
        private readonly ILogger<AlertController> _logger;
        private readonly IOptionsMonitor<GlobalConfigs> _globalConfigsMonitor;

        public AlertController(ISmsSender smsSender, IOptionsMonitor<GlobalConfigs> globalConfigsMonitor, ILogger<AlertController> logger)
        {
            _smsSender = smsSender;
            _logger = logger;
            _globalConfigsMonitor = globalConfigsMonitor;
        }

        [HttpGet("test")]
        public async Task<ActionResult> TestSms(string receiver, string message)
        {
            await _smsSender.SendSms(message, new[] { receiver });
            return Ok();
        }

        [HttpPost]
        public async Task Alert([FromBody] AlertmanagerPayload payload)
        {
            var tasks = new List<Task>();

            _logger.LogInformation($"sending sms for {payload.Alerts.Count} alerts");

            foreach (var alert in payload.Alerts)
            {
                var currentConfigs = _globalConfigsMonitor.CurrentValue;

                var recipients = currentConfigs.DefaultRecipients;
                if (alert.Annotations.ContainsKey("recipient-group") && currentConfigs.RecipientGroups.ContainsKey(alert.Annotations["recipient-group"]))
                {
                    recipients = currentConfigs.RecipientGroups[alert.Annotations["recipient-group"]];
                }

                var message = $"{alert.Status}{Environment.NewLine}{alert.Annotations["summary"]}:{Environment.NewLine}{alert.Annotations["description"]}";

                tasks.Add(_smsSender.SendSms(message, recipients.ToArray()));
            }

            await Task.WhenAll(tasks);
        }
    }
}
