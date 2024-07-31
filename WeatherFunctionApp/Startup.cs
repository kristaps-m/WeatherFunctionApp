using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using WeatherFunctionApp.Infrastructure.Services;

[assembly: FunctionsStartup(typeof(WeatherFunctionApp.Startup))]

namespace WeatherFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string openWeatherMapApiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");

            builder.Services.AddSingleton(new BlobService(storageConnectionString, "weatherdata"));
            builder.Services.AddSingleton(new TableService(storageConnectionString, "WeatherLogs"));
            builder.Services.AddHttpClient<WeatherService>();
        }
    }
}
