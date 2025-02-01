using System.Net.NetworkInformation;
using IoT.MqttBroker;
using IoT.MqttBroker.Contracts;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MQTTnet.Server;

namespace IoT.HealthCheck;

internal class IoTConnectionsHealthCheck : IHealthCheck
{
    private readonly IMediator _mediator;
    private readonly Dictionary<string, string>? _mqttClients;

    public IoTConnectionsHealthCheck(IConfiguration configuration,IMediator mediator)
    {
        _mediator = mediator;
        _mqttClients= configuration.GetSection("MQTT Clients").Get<Dictionary<string, string>>();
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var mqttClients = await _mediator.Send(new GetMqttBrokerClientsQuery(), cancellationToken);

        var errors = new Dictionary<string,object>();
        
        if (_mqttClients != null)
            foreach (var client in 
                     from client in _mqttClients
                     let result = FindClientByIpAddress(mqttClients, client.Key)
                     where result is null select client)
            {
                errors.Add(client.Key,new {MqttConnection = $"IP - {client.Value} is not connected to the mqtt broker",NetworkConnection = CheckNetworkConnection(client.Value)});
            }

        if (errors.Count == 0)
        {
            return HealthCheckResult.Healthy();
        }
        
        return HealthCheckResult.Unhealthy(
            "Services are not connected",
            null,
            errors);
    }
    
    private MqttClientStatus? FindClientByIpAddress(IList<MqttClientStatus> clientStatuses, string ipAddress)
    {
        if (_mqttClients != null && _mqttClients.TryGetValue(ipAddress, out var value))
        {
            return clientStatuses.FirstOrDefault(client => client.Endpoint.Contains(value));
        }

        return null;
    }

    private string CheckNetworkConnection(string ipAddress)
    {
        try
        {
            using Ping ping = new Ping();
            // Send a ping request with a timeout of 1000 ms
            PingReply? reply = ping.Send(ipAddress, 1000);

            // If the reply is null, it indicates a timeout or unreachable destination
            if (reply == null)
            {
                return $"{ipAddress} is unreachable (Ping timed out).";
            }

            // Check the reply status and return a message accordingly
            if (reply.Status == IPStatus.Success)
            {
                return $"{ipAddress} has network connection (Ping successful).";
            }

            return $"{ipAddress} ping failed: {reply.Status}";
        }
        catch (Exception ex)
        {
            return $"Ping failed: {ex.Message}";
        }
    }
}