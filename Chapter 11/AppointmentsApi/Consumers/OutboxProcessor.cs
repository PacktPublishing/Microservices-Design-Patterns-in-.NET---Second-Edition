using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Notification.Service;

public class OutboxProcessor(IServiceProvider _serviceProvider, ILogger<OutboxProcessor> _logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppointmentContext>();
            var messagePublisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var messages = await dbContext.OutboxMessages
                .Where(m => !m.Processed)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                try
                {
                    await messagePublisher.Publish(message.Payload);
                    message.Processed = true;
                    message.ProcessedOnUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox message with ID {MessageId}", message.Id);
                    // Optionally implement retry logic or move to a dead-letter queue
                }
            }

            await dbContext.SaveChangesAsync();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
