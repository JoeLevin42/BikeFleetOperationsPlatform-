using CsConsumer.Data;
using CsConsumer.Handlers;
using CsConsumer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;

var services = new ServiceCollection();

// SQL

var connectionString =
    Environment.GetEnvironmentVariable("CONNECTION_STRING");

services.AddDbContext<ApplicationDbContext>(
    options => options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))));

// Redis

var redisConnectionString =
    Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");

services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));

// MongoDB

var mongoConnectionString =
    Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

services.AddSingleton<IMongoClient>(
    new MongoClient(mongoConnectionString));

// Services

services.AddScoped<StationInformationService>();
services.AddScoped<StationStatusService>();
services.AddScoped<VehicleTypesService>();

// Handlers

services.AddScoped<StationInformationHandler>();
services.AddScoped<StationStatusHandler>();
services.AddScoped<VehicleTypesHandler>();

// Build DI container

var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    db.Database.EnsureCreated();
}

Console.WriteLine("CsConsumer started.");

Console.WriteLine("CsConsumer started.");

// Station Information

var stationInformationTask = Task.Run(() =>
{
    Console.WriteLine("Starting station information consumer...");

    using var scope = provider.CreateScope();

    var handler =
        scope.ServiceProvider
            .GetRequiredService<StationInformationHandler>();

    Console.WriteLine("Station information handler created.");

    handler.StationInforamtionConsume();
});

// Station Status

var stationStatusTask = Task.Run(async () =>
{
    Console.WriteLine("Starting station status consumer...");

    using var scope = provider.CreateScope();

    var handler =
        scope.ServiceProvider
            .GetRequiredService<StationStatusHandler>();

    Console.WriteLine("Station status handler created.");

    await handler.StationStatusConsume();
});

// Vehicle Types

var vehicleTypesTask = Task.Run(() =>
{
    Console.WriteLine("Starting vehicle types consumer...");

    using var scope = provider.CreateScope();

    var handler =
        scope.ServiceProvider
            .GetRequiredService<VehicleTypesHandler>();

    Console.WriteLine("Vehicle types handler created.");

    handler.VehicleTypesConsume();
});

// Run all three consumers

await Task.WhenAll(
    stationInformationTask,
    stationStatusTask,
    vehicleTypesTask);