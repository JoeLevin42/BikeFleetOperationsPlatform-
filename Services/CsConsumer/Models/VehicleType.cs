using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CsConsumer.Models;

public class VehicleTypesResponse
{
    [JsonPropertyName("data")]
    public VehicleTypesData Data { get; set; } = new();
}

public class VehicleTypesData
{
    [JsonPropertyName("vehicle_types")]
    public List<VehicleType> VehicleTypes { get; set; } = new();
}

public class VehicleType
{
    
    [JsonPropertyName("vehicle_type_id")]
    [Required]
    public string VehicleTypeId { get; set; } = "";

    [JsonPropertyName("form_factor")]
    public string FormFactor { get; set; } = "";

    [JsonPropertyName("propulsion_type")]
    public string PropulsionType { get; set; } = "";

    [JsonPropertyName("max_range_meters")]
    public double? MaxRangeMeters { get; set; }
}