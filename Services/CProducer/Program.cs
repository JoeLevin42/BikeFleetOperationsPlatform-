//Creating here the DI container 
using CProducer.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddHttpClient();

services.AddScoped<StationInformationService>();
services.AddScoped<StationStatusService>();
services.AddScoped<VehicleTypesService>();

var serviceProvider = services.BuildServiceProvider();

