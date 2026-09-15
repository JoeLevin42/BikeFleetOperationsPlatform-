//Creating here the DI container 
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddHttpClient();

var serviceProvider = services.BuildServiceProvider();