using BlogPhone.BackgroundServices.Messages;
using BlogPhone.Models.LogViewer;
using BlogPhone.Models.LogViewer.Database;

namespace BlogPhone.BackgroundServices
{
    public class LogSaver : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Info.Print("LogSaver", $"UTC: {DateTimeOffset.UtcNow.ToString("G")}");
                
                using (LogContext context = new LogContext())
                {
                    if (LogActions.Logs.Count > 0)
                    {
                        await context.LogStrings.AddRangeAsync(LogActions.Logs);
                        await context.SaveChangesAsync();

                        LogActions.Logs.Clear();
                    }
                }
                Info.Print("________________________________________________");
                await Task.Delay(30000, stoppingToken); // every min
            }
        }
    }
}
