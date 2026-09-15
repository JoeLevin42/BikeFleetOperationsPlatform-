using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CProducer.Models;

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
    [Required]
    public string StationId { get; set; }


    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lat")]
    [Range(-90.0,90.0)]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    [Range(-180.0,180.0)]
    public double Lon { get; set; }

    [JsonPropertyName("capacity")]
    [Range(0, int.MaxValue)]
    public int Capicity { get; set; }
}
