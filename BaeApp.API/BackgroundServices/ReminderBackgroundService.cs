using BaeApp.Core.Interfaces;

namespace BaeApp.API.BackgroundServices
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IReminderService _reminderService;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(IReminderService reminderService, ILogger<ReminderBackgroundService> logger)
        {
            _reminderService = reminderService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReminderBackgroundService is starting. ");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _reminderService.ProcessPendingAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when processing reminders");
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            _logger.LogInformation("ReminderBackgroundService is Stopping");
        }
    }
}
