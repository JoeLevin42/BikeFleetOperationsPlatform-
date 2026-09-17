using CsConsumer.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;

namespace CsConsumer.Services;

public class StationStatusService
{
    private readonly IDatabase _redis;
    private readonly IMongoCollection<StationStatus> _mongoCollection;

    public StationStatusService(
        IConnectionMultiplexer redis,
        IMongoClient mongoClient)
    {
        _redis = redis.GetDatabase();

        var database = mongoClient.GetDatabase("DeveloperLearning");

        _mongoCollection = database.GetCollection<StationStatus>(
            "station_status");
    }

    public async Task<bool> ProcessStationStatus(StationStatus status)
    {
        if (!ValidateStatus(status))
        {
            return false;
        }

        string key = $"station-status:{status.StationId}";

        var previousState = await _redis.StringGetAsync(key);

        if (previousState.HasValue)
        {
            var previousStatus =
                JsonSerializer.Deserialize<StationStatus>(
                    previousState!);

            if (previousStatus != null &&
                !HasChanged(previousStatus, status))
            {
                return false;
            }
        }

        await _mongoCollection.InsertOneAsync(status);

        var newState = JsonSerializer.Serialize(status);

        await _redis.StringSetAsync(key, newState);

        return true;
    }

    private bool ValidateStatus(StationStatus status)
    {
        if (status == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(status.StationId))
        {
            return false;
        }

        if (status.NumBikesAvailable < 0)
        {
            return false;
        }

        if (status.NumDocksAvailable < 0)
        {
            return false;
        }

        return true;
    }

    private bool HasChanged(
        StationStatus previousStatus,
        StationStatus newStatus)
    {
        return previousStatus.NumBikesAvailable !=
                   newStatus.NumBikesAvailable
               || previousStatus.NumDocksAvailable !=
                   newStatus.NumDocksAvailable
               || previousStatus.IsRenting !=
                   newStatus.IsRenting
               || previousStatus.IsReturning !=
                   newStatus.IsReturning;
    }
}