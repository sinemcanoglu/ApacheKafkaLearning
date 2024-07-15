using Confluent.Kafka;

namespace Kafka.Shared
{
    public class EventBus : IEventBus
    {
        private readonly IProducer<Null, string> _producer;
        private readonly IConsumer<Null, string> _consumer;
        private readonly string _topic;

        public EventBus(KafkaSettings settings)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = settings.BootstrapServers };
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = settings.AutoOffsetReset switch
                {
                    "Earliest" => AutoOffsetReset.Earliest,
                    "Latest" => AutoOffsetReset.Latest,
                    "Error" => AutoOffsetReset.Error,
                    _ => AutoOffsetReset.Earliest
                },
                EnableAutoCommit = settings.EnableAutoCommit,
                StatisticsIntervalMs = settings.StatisticsIntervalMs
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _topic = settings.TopicName;
        }

        public void Produce(string message)
        {
            _producer.Produce(_topic, new Message<Null, string> { Value = message });
            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public void Consume(Action<string> messageHandler, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    messageHandler(consumeResult.Message.Value);
                }
            }
            catch (OperationCanceledException)
            {
                // 
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
