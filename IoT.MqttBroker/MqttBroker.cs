using System.Text;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Serilog;

namespace IoT.MqttBroker;


//TODO : Use IP whitelisting to restrict who can access the broker.
internal class MqttBroker(ILogger logger,MqttServer server,ClientAuthenticator clientAuthenticator)
{
    // Event handler when a client connects
    public Task OnClientConnected(ClientConnectedEventArgs eventArgs)
    {
        logger.Information($"Client '{eventArgs.ClientId}' connected.");
        return Task.CompletedTask;
    }

    // Event handler when a connection is validated
    public Task ValidateConnection(ValidatingConnectionEventArgs eventArgs)
    {
        //TODO : Authenticate not secured connections.
        if (eventArgs.IsSecureConnection)
        {
            return clientAuthenticator.Authenticate(eventArgs.UserName,eventArgs.Password) ?
                Task.CompletedTask : server.DisconnectClientAsync(eventArgs.ClientId,MqttDisconnectReasonCode.NotAuthorized);
        }

        logger.Information("Endpoint : " + eventArgs.Endpoint);
        return Task.CompletedTask;

    }

    // Event handler for receiving messages
    public Task OnApplicationMessageReceivedAsync(InterceptingPublishEventArgs eventArgs)
    {
        // Get the topic and payload of the received message
        var topic = eventArgs.ApplicationMessage.Topic;
        var payload = eventArgs.ApplicationMessage.PayloadSegment;
        var payloadString = Encoding.UTF8.GetString(payload);

        //TODO: Dont know if i actually need it.Can just use cache.Need to read how it works,maybe it is the same.Good option
        server.UpdateRetainedMessageAsync(eventArgs.ApplicationMessage);

        logger.Information($"Message received on topic '{topic}': {payloadString}");
        
        return Task.CompletedTask;
    }

    public async Task PublishMessage(string topic,string payload)
    {
        var message = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(payload).Build();
        
        await server.InjectApplicationMessage(
            new InjectedMqttApplicationMessage(message)
            {
                SenderClientId = "MQTT Broker",
            });
    }
    
    
}