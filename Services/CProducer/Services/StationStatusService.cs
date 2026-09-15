namespace CProducer.Services;

using CProducer.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public class StationStatusService
{
    private readonly IHttpClientFactory _factory;

    public StationStatusService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private async Task<string> ReadFromWeb()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_status.json");

        response.EnsureSuccessStatusCode(); //this throws httpExeption

        var json = await response.Content.ReadAsStringAsync();

        return json;
    }

    public async Task<IEnumerable<StationStatus>> ProcessData()
    {
        var json = await ReadFromWeb();

        var jsonObj = JsonSerializer.Deserialize<StationStatusResponse>(json);

        var stations = jsonObj?.Data?.Stations
                       ?? Enumerable.Empty<StationStatus>();

        var validStations = new List<StationStatus>();

        foreach (var station in stations)
        {
            var context = new ValidationContext(station);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                station,
                context,
                results,
                validateAllProperties: true
            );

            if (isValid)
            {
                validStations.Add(station);
            }
            else
            {
                // invadid so rejected
                foreach (var error in results)
                {
                    Console.WriteLine($"Invalid station: {error.ErrorMessage}");
                }
            }
        }

        return validStations;
    }
}

