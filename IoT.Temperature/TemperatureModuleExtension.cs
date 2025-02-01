using IoT.Temperature.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Serilog;

namespace IoT.Temperature;

public static class TemperatureModuleExtension
{
    
    public static IServiceCollection AddTemperatureModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<System.Reflection.Assembly> mediatRAssemblies)
    
    {
        services.Configure<ModuleDatabaseSettings>(
            config.GetSection("TemperatureModuleDatabase"));
        
        services.AddSingleton<TemperatureService>();
        
        BsonSerializer.RegisterSerializer(typeof(DateTime), new LocalDateTimeSerializer());
        
        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(TemperatureModuleExtension).Assembly);
        
        logger.Information("{Module} module services registered", "Temperature");

        return services;
    }
}