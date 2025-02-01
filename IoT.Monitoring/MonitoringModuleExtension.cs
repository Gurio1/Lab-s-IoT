using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IoT.Monitoring;


//IDK if i need for this separate module
public static class MonitoringModuleExtension
{
    public static IServiceCollection AddMonitoringModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<System.Reflection.Assembly> mediatRAssemblies)
    {
        
        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(MonitoringModuleExtension).Assembly);
        
        logger.Information("{Module} module services registered", "Monitoring");

        return services;
    }
    
    public static IApplicationBuilder UseMonitoringModule(this IApplicationBuilder app,ILogger logger)
    {
        return app;
    } 
}