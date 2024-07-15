using Kafka.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Consumer
{
    public class ConsumerService : BackgroundService
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(IEventBus eventBus, ILogger<ConsumerService> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consumer is starting.");

            stoppingToken.Register(() => _logger.LogInformation("Consumer is stopping."));

            try
            {
                _eventBus.Consume(message =>
                {
                    _logger.LogInformation($"Consumed: {message}");
                }, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer is stopping due to cancellation.");
            }

            await Task.CompletedTask;
        }
    }
}
