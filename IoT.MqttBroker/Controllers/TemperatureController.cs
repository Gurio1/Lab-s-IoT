using System.Text;
using System.Text.Json;
using IoT.MqttBroker.Contracts;
using IoT.MqttBroker.Contracts.DTO;
using MediatR;
using MQTTnet.AspNetCore.Routing;
using Serilog;

namespace IoT.MqttBroker.Controllers;

[MqttController]
internal class TemperatureController(ILogger logger,IMediator mediator) : MqttBaseController
{
    [MqttRoute("temperature/server")]
    public async Task ReceiveServerTemperature()
    {
        var payloadString = Encoding.UTF8.GetString(Message.PayloadSegment);

        var temperature = JsonSerializer.Deserialize<ServerTemperatureData>(payloadString);

        if (temperature is null)
        {
            logger.Warning("{Topic} received empty or wrong data - {payload}","temperature/server",payloadString);
            return;
        }
        
        await mediator.Publish(new TemperatureDataReceivedEvent("temperature/server",temperature));
        
    }
}