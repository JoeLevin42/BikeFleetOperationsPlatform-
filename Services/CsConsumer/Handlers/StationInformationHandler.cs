using System.Text.Json;
using Confluent.Kafka;
using CsConsumer.Models;
using CsConsumer.Services;

namespace CsConsumer.Handlers;

public class StationInformationHandler
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly StationInformationService _service;

    public StationInformationHandler(
        StationInformationService service)
    {
        _service = service;

        var broker =
            Environment.GetEnvironmentVariable("KAFKA_BROKER")
            ?? "localhost:9092";

        var config = new ConsumerConfig
        {
            BootstrapServers = broker,
            GroupId = "station-information-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer =
            new ConsumerBuilder<Null, string>(config).Build();
    }

    public void StationInforamtionConsume()
    {
        var topic =
            Environment.GetEnvironmentVariable(
                "STATION_INFORMATION_TOPIC")
            ?? "bike.station-information";

        _consumer.Subscribe(topic);

        while (true)
        {
            var result = _consumer.Consume();

            var station =
                JsonSerializer.Deserialize<StationInformation>(
                    result.Message.Value);

            if (station == null)
            {
                Console.WriteLine(
                    "Failed to deserialize station information.");

                continue;
            }

            var success =
                _service.ProcessStation(station);

            Console.WriteLine(success
                ? $"Station {station.StationId} processed successfully."
                : $"Station {station.StationId} was ignored.");
        }
    }
}