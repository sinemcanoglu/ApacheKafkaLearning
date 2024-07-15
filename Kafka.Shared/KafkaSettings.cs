namespace Kafka.Shared
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string TopicName { get; set; }
        public string GroupId { get; set; }
        public string AutoOffsetReset { get; set; } // Earliest, Latest, None
        public bool EnableAutoCommit { get; set; }
        public int StatisticsIntervalMs { get; set; }
    }
}
