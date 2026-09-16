// need to create the DI Container 
using CsConsumer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



var services = new ServiceCollection();

IConfigurationBuilder builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional:true)
    .AddEnvironmentVariables();

IConfigurationRoot configuration = builder.Build();

var connectionString =
    configuration.GetConnectionString("DefaultConnection"); // need be assigent in the appsettings


services.AddDbContext<ApplicationDbContext>(
          dbContextOptions => dbContextOptions
              .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


var serviceProvider = services.BuildServiceProvider();
