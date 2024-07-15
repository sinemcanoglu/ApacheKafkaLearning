using Kafka.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


            var services = new ServiceCollection();
            services.AddSingleton(configuration.GetSection("KafkaSettings").Get<KafkaSettings>());
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IHostedService, ConsumerService>();
            services.AddLogging(configure => configure.AddConsole());

            var serviceProvider = services.BuildServiceProvider();
           
            var hostedService = serviceProvider.GetService<IHostedService>();
            hostedService.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
                               
            Thread.Sleep(Timeout.Infinite);
        }
    }
}