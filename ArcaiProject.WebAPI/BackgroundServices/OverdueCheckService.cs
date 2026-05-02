using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ArcaiProject.Business.Interfaces;

namespace ArcaiProject.WebAPI.BackgroundServices
{
    public class OverdueCheckService : IHostedService, IDisposable
    {
        private readonly ILogger<OverdueCheckService> _logger;
        private Timer? _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public OverdueCheckService(ILogger<OverdueCheckService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Overdue Check Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, _checkInterval);
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Overdue Check Service is running.");

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArcaiDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var now = DateTime.UtcNow;

                var overdueRecords = await dbContext.BorrowingRecords
                    .Include(r => r.RequesterUser)
                    .Include(r => r.Document)
                    .Where(r => r.Status == BorrowingRecordStatus.CheckedOut && r.DueDate.HasValue && r.DueDate.Value < now)
                    .ToListAsync();

                if (overdueRecords.Any())
                {
                    _logger.LogInformation("Found {count} overdue records. Updating status and creating notifications...", overdueRecords.Count);

                    var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Role == "Admin");

                    foreach (var record in overdueRecords)
                    {
                        record.Status = BorrowingRecordStatus.Overdue;

                        if (record.RequesterUser != null && record.Document != null)
                        {
                            await notificationService.CreateNotificationAsync(
                                record.RequesterUser.Id,
                                $"[REMINDER] The return date for the document '{record.Document.Title}' ({record.DueDate:dd.MM.yyyy}) has passed. Please return it as soon as possible.",
                                $"/borrowing/{record.Id}");
                        }
                    }

                    if (adminUser != null)
                    {
                        await notificationService.CreateNotificationAsync(
                            adminUser.Id,
                            $"The system has detected {overdueRecords.Count} overdue document(s) and updated their status to 'Overdue'.",
                            "/borrowing/borrowed");
                    }

                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated overdue records and created notifications.");
                }
                else
                {
                    _logger.LogInformation("No overdue records found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking for overdue records.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Overdue Check Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
