

using Confluent.Kafka;
using CsConsumer.Data;
using CsConsumer.Models;
using CsConsumer.Services;
using System.Text.Json;

namespace CsConsumer.Handlers;


public class StationInformationHandler
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly StationInformationService _service;


    public StationInformationHandler()
    {
        var broker = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "localhost:9092";
        var config = new ConsumerConfig
        {
            BootstrapServers = broker,
            GroupId = "station-information-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }
    
    public void StationInforamtionConsume()
    {   
        _consumer.Subscribe("bike.station-information"); 
        //right now its will be hardcoded later its will get from env

        
        while (true)
        {
            var result = _consumer.Consume();

            var response = JsonSerializer.Deserialize<StationInfoResponse>(
                result.Message.Value);

            if (response == null)
            {
                Console.WriteLine("Failed to desrialize station information");
                continue;
            }

            bool success = _service.ProcessStations(response);

            if (success)
            {
                Console.WriteLine("Station information processed successfully");
            }
            else
            {
                Console.WriteLine("Failed to procees station information");
            }

        }
    }



}