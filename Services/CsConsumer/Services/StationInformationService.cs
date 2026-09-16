using CsConsumer.Data;
using CsConsumer.Models;

namespace CsConsumer.Services;

public class StationInformationService
{
    private readonly ApplicationDbContext _context;

    public StationInformationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool ProcessStations(StationInfoResponse response)
    {
        foreach (var station in response.Data.Stations)
        {
            if (!ValidateStation(station))
            {
                return false;
            }

            var existingStation = GetStation(station.StationId);

            if (existingStation == null)
            {
                AddStation(station);
            }
            else if (HasChanged(existingStation, station))
            {
                UpdateStation(existingStation, station);
            }
        }

        _context.SaveChanges();

        return true;
    }

    private bool ValidateStation(StationInformation station)
    {
        return !string.IsNullOrWhiteSpace(station.StationId);
    }

    private StationInformation? GetStation(string stationId)
    {
        return _context.StationInformation
            .FirstOrDefault(x => x.StationId == stationId);
    }
        
    private bool HasChanged(
        StationInformation existingStation,
        StationInformation newStation)
    {
        return existingStation.Name != newStation.Name ||
               existingStation.Lat != newStation.Lat ||
               existingStation.Lon != newStation.Lon ||
               existingStation.Capacity != newStation.Capacity;
    }

    private void AddStation(StationInformation station)
    {
        _context.StationInformation.Add(station);
    }

    private void UpdateStation(
        StationInformation existingStation,
        StationInformation newStation)
    {
        existingStation.Name = newStation.Name;
        existingStation.Lat = newStation.Lat;
        existingStation.Lon = newStation.Lon;
        existingStation.Capacity = newStation.Capacity;
    }
}