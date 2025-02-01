using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Routing;

namespace IoT.MqttBroker;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

public static class MqttBrokerExtension
{
    public static IServiceCollection AddMqttBrokerServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<System.Reflection.Assembly> mediatRAssemblies)
    
    {
        
        var certificate = new X509Certificate2("C:/Users/Andrey-PC/Desktop/LAB/test.pfx","localhost", X509KeyStorageFlags.Exportable);
        
        //MQTT brokers generally provide TLS encryption for secure communication between clients and brokers. The default secure MQTT broker ports are 8883 for MQTT 
        services.AddHostedMqttServer(mqttServer =>
            {
                mqttServer
                    .WithEncryptionCertificate(certificate)
                    .WithEncryptedEndpoint()
                    .Build(); // the secured port
            })
            .AddMqttConnectionHandler()
            .AddConnections()
            .AddMqttControllers([typeof(MqttBrokerExtension).Assembly]);
        
        
        services.AddScoped<MqttBroker>();
        services.Configure<ClientsCredentialOptions>(
            config.GetSection(ClientsCredentialOptions.Position));
        services.AddScoped<ClientAuthenticator>();

        
        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(MqttBrokerExtension).Assembly);
        
        logger.Information("{Module} module services registered", "Mqtt Broker");

        return services;
    }

    
    //TODO : Review this.Dont like separating pipeline registration.Seems very bad
    public static IApplicationBuilder UseMqttBroker(this IApplicationBuilder app,IServiceProvider serviceProvider,ILogger logger)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapConnectionHandler<MqttConnectionHandler>(
                "/mqtt",
                httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                    protocolList =>
                        protocolList.FirstOrDefault() ?? string.Empty);
        });
        
        using var serviceScope = serviceProvider.CreateScope();
        
        var services = serviceScope.ServiceProvider;
        
        app.UseMqttServer(server =>
        {
            var mqttController = services.GetRequiredService<MqttBroker>();
            
            server.ValidatingConnectionAsync += mqttController.ValidateConnection;
            server.ClientConnectedAsync += mqttController.OnClientConnected;
            server.InterceptingPublishAsync += mqttController.OnApplicationMessageReceivedAsync;
            
            server.WithAttributeRouting(serviceProvider);

        });

        return app;
    } 
}