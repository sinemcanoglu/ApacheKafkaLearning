namespace Kafka.Shared
{
    public interface IEventBus
    {
        void Produce(string message);
        void Consume(Action<string> messageHandler, CancellationToken cancellationToken);
    }
}
