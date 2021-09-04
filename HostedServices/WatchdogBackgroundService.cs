using System;
using System.Threading;
using System.Threading.Tasks;
using AlertmanagerSmsNotifier.Controllers;
using AlertmanagerSmsNotifier.Models.Configs;
using AlertmanagerSmsNotifier.Services.Interfaces;
using Humanizer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class WatchdogBackgroundService : BackgroundService
{

  private readonly ISmsSender _smsSender;
  private readonly ILogger<AlertController> _logger;
  private readonly IOptionsMonitor<GlobalConfigs> _globalConfigsMonitor;

  public static DateTime? LastWatchdogEventReceived = null;

  public WatchdogBackgroundService(ISmsSender smsSender, IOptionsMonitor<GlobalConfigs> globalConfigsMonitor, ILogger<AlertController> logger)
  {
    _smsSender = smsSender;
    _logger = logger;
    _globalConfigsMonitor = globalConfigsMonitor;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      if (LastWatchdogEventReceived.HasValue)
      {
        if (DateTime.Now - LastWatchdogEventReceived.Value > TimeSpan.FromMinutes(1))
        {
          var currentConfigs = _globalConfigsMonitor.CurrentValue;
          var recipients = currentConfigs.DefaultRecipients;
          await _smsSender.SendSms($"watchdog event is missing since {LastWatchdogEventReceived.Value.Humanize()}", recipients.ToArray());
          await Task.Delay(TimeSpan.FromSeconds(50), stoppingToken);
        }
      }

      await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
  }
}