using CsConsumer.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace CsConsumer.Services;

public class StationStatusService
{
    private readonly IDatabase _redis;

    public StationStatusService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task<bool> ProcessStationStatus(StationStatus status)
    {
        // 1. Validate the status
        if (!ValidateStatus(status))
        {
            return false;
        }

        // 2. Create Redis key for this station
        string key = $"station-status:{status.StationId}";

        // 3. Get previous state from Redis
        var previousState = await _redis.StringGetAsync(key);

        // 4. If there is a previous state, compare it
        if (previousState.HasValue)
        {
            var previousStatus =
                JsonSerializer.Deserialize<StationStatus>(
                    previousState!);

            if (previousStatus != null &&
                !HasChanged(previousStatus, status))
            {
                // Nothing changed
                return false;
            }
        }

        // 5. State is new or changed
        // MongoDB saving will happen after this.

        // 6. Update Redis with the new state
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