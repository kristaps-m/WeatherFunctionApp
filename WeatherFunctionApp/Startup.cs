using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using WeatherFunctionApp.Core.Interfaces;
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

            builder.Services.AddSingleton<IBlobService>(new BlobService(storageConnectionString, "weatherdata"));
            builder.Services.AddSingleton<ITableService>(new TableService(storageConnectionString, "WeatherLogs"));
            builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }
    }
}
