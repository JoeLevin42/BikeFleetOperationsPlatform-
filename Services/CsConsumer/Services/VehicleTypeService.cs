using CsConsumer.Data;
using CsConsumer.Models;

namespace CsConsumer.Services;

public class VehicleTypesService
{
    private readonly ApplicationDbContext _context;

    public VehicleTypesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool ProcessVehicleType(VehicleType vehicleType)
    {
        if (!ValidateVehicleType(vehicleType))
        {
            return false;
        }

        var existingVehicleType =
            GetVehicleType(vehicleType.VehicleTypeId);

        if (existingVehicleType == null)
        {
            AddVehicleType(vehicleType);
        }
        else if (HasChanged(
            existingVehicleType,
            vehicleType))
        {
            UpdateVehicleType(
                existingVehicleType,
                vehicleType);
        }

        _context.SaveChanges();

        return true;
    }

    private bool ValidateVehicleType(
        VehicleType vehicleType)
    {
        return !string.IsNullOrWhiteSpace(
            vehicleType.VehicleTypeId);
    }

    private VehicleType? GetVehicleType(
        string vehicleTypeId)
    {
        return _context.VehicleType
            .FirstOrDefault(x =>
                x.VehicleTypeId == vehicleTypeId);
    }

    private bool HasChanged(
        VehicleType existingVehicleType,
        VehicleType newVehicleType)
    {
        return existingVehicleType.FormFactor !=
                   newVehicleType.FormFactor
               || existingVehicleType.PropulsionType !=
                   newVehicleType.PropulsionType
               || existingVehicleType.MaxRangeMeters !=
                   newVehicleType.MaxRangeMeters;
    }

    private void AddVehicleType(
        VehicleType vehicleType)
    {
        _context.VehicleType.Add(vehicleType);
    }

    private void UpdateVehicleType(
        VehicleType existingVehicleType,
        VehicleType newVehicleType)
    {
        existingVehicleType.FormFactor =
            newVehicleType.FormFactor;

        existingVehicleType.PropulsionType =
            newVehicleType.PropulsionType;

        existingVehicleType.MaxRangeMeters =
            newVehicleType.MaxRangeMeters;
    }
}