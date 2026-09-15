namespace CProducer.Services;

public class ProducerService
{
    private readonly StationStatusService _stationStatusService;
    private readonly StationInformationService _stationInformationSerive;
    private readonly VehicleTypesService _vehicleTypesService;

    public ProducerService(
        StationStatusService stationStatusService,
        StationInformationService stationInformationSerive,
        VehicleTypesService vehicleTypesService)
    {
        _stationStatusService = stationStatusService;
        _stationInformationSerive = stationInformationSerive;
        _vehicleTypesService = vehicleTypesService;
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

        //TODO send to kafka


        Console.WriteLine($"Station status {stations.Count()} stations");
    }

    private async Task ProduceStationInformation()
    {
        var stations = await _stationInformationSerive.ProcessData();


        //TODO send to kafka
        Console.WriteLine($"Stations information {stations.Count()} stations");
    }

    private async Task ProduceVehicleTypes()
    {
        var vehicles = await _vehicleTypesService.ProcessData();

        //Todo send to kafka

        Console.WriteLine($"Vehicle types: {vehicles.Count()} vehicles");

    }




}
