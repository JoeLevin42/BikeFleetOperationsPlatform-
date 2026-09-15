namespace CProducer.Services;
using CProducer.Models;
using System.Text.Json;

public class StationInformationService
{
    private readonly IHttpClientFactory _factory;

    public StationInformationService(IHttpClientFactory factory)
    {
        _factory = factory; 
    }

    private async Task<string> ReadFromWeb()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_information.json");

        response.EnsureSuccessStatusCode(); //this throws httpExeption

        var json = await response.Content.ReadAsStringAsync();

        return json;
    }

    public async Task<IEnumerable<StationInformation> ProcessData()
    {
        var json = await ReadFromWeb();

        var jsonObjList = JsonSerializer.Deserialize<List<StationInformation>>(json);


        return jsonObjList;

    }
}

