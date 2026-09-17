using Confluent.Kafka;
using System.Text.Json;


namespace Cproducer.Services;

public class KafkaProducerService
{
    private readonly IProducer<Null, string> _producer;

    
    public KafkaProducerService()
    {
        var broker = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "localhost:9092"; //maybe later 
        var config = new ProducerConfig
        {

            BootstrapServers = broker
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();      
    }

public async Task ProduceJObjAsync<T>(string topic, T data)
    {
        var json = JsonSerializer.Serialize(data);

        await _producer.ProduceAsync(
            topic,
            new Message<Null, string>
            {
                Value = json
            });
    }
}