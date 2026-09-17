using Confluent.Kafka;
using CsConsumer.Models;
using CsConsumer.Services;
using System.Text.Json;

namespace CsConsumer.Handlers;

public class VehicleTypesHandler
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly VehicleTypesService _service;

    public VehicleTypesHandler(VehicleTypesService service)
    {
        _service = service;

        var broker = Environment.GetEnvironmentVariable("KAFKA_BROKER")
                     ?? "localhost:9092";

        var config = new ConsumerConfig
        {
            BootstrapServers = broker,
            GroupId = "vehicle-types-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    public void VehicleTypesConsume()
    {
        var topic = Environment.GetEnvironmentVariable(
            "VEHICLE_TYPES_TOPIC")
            ?? "bike.vehicle-types";

        _consumer.Subscribe(topic);

        while (true)
        {
            var result = _consumer.Consume();

            var vehicleType =
                JsonSerializer.Deserialize<VehicleType>(
                    result.Message.Value);

            if (vehicleType == null)
            {
                Console.WriteLine(
                    "Failed to deserialize vehicle type.");

                continue;
            }

            bool success =
                _service.ProcessVehicleType(vehicleType);

            if (success)
            {
                Console.WriteLine(
                    $"Vehicle type {vehicleType.VehicleTypeId} processed successfully.");
            }
            else
            {
                Console.WriteLine(
                    $"Vehicle type {vehicleType.VehicleTypeId} was ignored.");
            }
        }
    }
}