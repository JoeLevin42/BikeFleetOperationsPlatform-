using System.Text.Json.Serialization;

namespace CsConsumer.Models;

public class StationInfoResponse
{
    [JsonPropertyName("data")]
    public FeedData Data { get; set; }

    public class FeedData
    {
        [JsonPropertyName("stations")]
        public List<StationInformation> Stations { get; set; }
    }
}

public class StationInformation
{
    [JsonPropertyName("station_id")]
    public string StationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
}