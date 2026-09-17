using Cproducer.Services;
using CProducer.Models;
using System.Text.Json;

namespace CProducer.Services;

public class ProducerService
{
    private readonly StationStatusService _stationStatusService;
    private readonly StationInformationService _stationInformationSerive;
    private readonly VehicleTypesService _vehicleTypesService;
    private readonly KafkaProducerService _kafkaProducerService;

    public ProducerService(
        StationStatusService stationStatusService,
        StationInformationService stationInformationSerive,
        VehicleTypesService vehicleTypesService,
        KafkaProducerService kafkaProducerService)
    {
        _stationStatusService = stationStatusService;
        _stationInformationSerive = stationInformationSerive;
        _vehicleTypesService = vehicleTypesService;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task Run()
    {
        var stationStatusTask = RunStationStatus();
        var hourlyFeedsTask = RunHourlyFeeds();

        await Task.WhenAll(stationStatusTask, hourlyFeedsTask);
    }

    private async Task RunHourlyFeeds()
    {
        //first call the services immediately
        await ProduceStationInformation();
        await ProduceVehicleTypes();

        using var timer = new
            PeriodicTimer(TimeSpan.FromHours(1));

        while (await timer.WaitForNextTickAsync())
        {
            await ProduceStationInformation();
            await ProduceVehicleTypes();
        }
    }

    private async Task RunStationStatus()
    {
        //first all immediately
        await ProduceStationStatus();

        using var timer = new
            PeriodicTimer(TimeSpan.FromSeconds(60));

        while (await timer.WaitForNextTickAsync())
        {
            await ProduceStationStatus();
        }
    }
    
    private async Task ProduceStationStatus()
    {
        //TODO : send stations to Kafka

        var stations = await _stationStatusService.ProcessData();

        foreach (var station in stations)
        {
            await _kafkaProducerService.ProduceJObjAsync<StationStatus>("bike.station-status", station);
        }
        

        Console.WriteLine($"Station status {stations.Count()} stations");
    }

    private async Task ProduceStationInformation()
    {
        var stations = await _stationInformationSerive.ProcessData();



        foreach (var stationInfo in stations)
        {
            await _kafkaProducerService.ProduceJObjAsync<StationInformation>("bike.station-information", stationInfo);
        }
        Console.WriteLine($"Stations information {stations.Count()} stations");
    }

    private async Task ProduceVehicleTypes()
    {
        var vehicles = await _vehicleTypesService.ProcessData();

        foreach (var vehicle in vehicles)
        {
            await _kafkaProducerService.ProduceJObjAsync<VehicleType>("bike.vehicle-types", vehicle);
        }

        Console.WriteLine($"Vehicle types: {vehicles.Count()} vehicles");

    }




}
