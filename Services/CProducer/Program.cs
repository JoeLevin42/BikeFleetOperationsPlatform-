using Cproducer.Services;
using CProducer.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddHttpClient();

services.AddScoped<StationInformationService>();
services.AddScoped<StationStatusService>();
services.AddScoped<VehicleTypesService>();
services.AddScoped<KafkaProducerService>();
services.AddScoped<ProducerService>();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var producerService =
    scope.ServiceProvider.GetRequiredService<ProducerService>();

await producerService.Run();