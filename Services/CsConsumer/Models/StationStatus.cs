using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static System.Collections.Specialized.BitVector32;

namespace CsConsumer.Models;

public class StationStatusResponse
{
    [JsonPropertyName("data")]
    public FeedData Data { get; set; } = new();

    public class FeedData
    {
        [JsonPropertyName("stations")]
        public List<StationStatus> Stations { get; set; } = new();
    }
}

public class StationStatus
{
    [JsonPropertyName("station_id")]
    [Required]
    [Key]
    public string StationId { get; set; }

    [JsonPropertyName("num_bikes_available")]
    [Range(0, int.MaxValue)]
    public int NumBikesAvailable { get; set; }

    [JsonPropertyName("num_docks_available")]
    [Range(0, int.MaxValue)]
    public int NumDocksAvailable { get; set; }

    [JsonPropertyName("is_renting")]
    public int IsRenting { get; set; }

    [JsonPropertyName("is_returning")]
    public int IsReturning { get; set; }


    [JsonPropertyName("last_reported")]
    public long LastReported { get; set; }
}


