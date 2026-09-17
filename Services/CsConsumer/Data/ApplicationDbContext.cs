

using CsConsumer.Data;
using CsConsumer.Models;
using CsConsumer.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CsConsumer.Data;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<StationInformation> StationInformation { get; set; }
    public DbSet<VehicleType> VehicleType { get; set; }
}
