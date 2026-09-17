using System.Text.Json;
using Confluent.Kafka;
using CsConsumer.Models;
using CsConsumer.Services;

namespace CsConsumer.Handlers;

public class StationStatusHandler
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly StationStatusService _service;

    public StationStatusHandler(StationStatusService service)
    {
        _service = service;

        var broker = Environment.GetEnvironmentVariable("KAFKA_BROKER")
                     ?? "localhost:9092";

        var config = new ConsumerConfig
        {
            BootstrapServers = broker,
            GroupId = "station-status-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    public async Task StationStatusConsume()
    {
        var topic = Environment.GetEnvironmentVariable(
            "STATION_STATUS_TOPIC")
            ?? "bike.station-status";

        _consumer.Subscribe(topic);

        while (true)
        {
            var result = _consumer.Consume();

            var status =
                JsonSerializer.Deserialize<StationStatus>(
                    result.Message.Value);

            if (status == null)
            {
                Console.WriteLine(
                    "Failed to deserialize station status.");

                continue;
            }

            var processed =
                await _service.ProcessStationStatus(status);

            if (processed)
            {
                Console.WriteLine(
                    $"Station {status.StationId} status changed and was processed.");
            }
            else
            {
                Console.WriteLine(
                    $"Station {status.StationId} status was ignored.");
            }
        }
    }
}