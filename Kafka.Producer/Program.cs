using Kafka.Shared;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        KafkaSettings kafkaSettings = configuration.GetSection("KafkaSettings").Get<KafkaSettings>();

        IEventBus eventBus = new EventBus(kafkaSettings);
               
        int counter = 0;   

        try
        {
            while (!new CancellationTokenSource().Token.IsCancellationRequested)
            {
                string message = $"Message {counter++}";
                eventBus.Produce(message);
                Console.WriteLine($"Produced: {message}");
                await Task.Delay(1000, new CancellationTokenSource().Token);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Producer stopped.");
        }
    }
}
