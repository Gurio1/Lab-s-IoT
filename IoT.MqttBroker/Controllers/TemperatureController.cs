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
        
        //var temperature = JsonSerializer.Deserialize<ServerTemperatureData>(payloadString);
        
        var data = JsonSerializer.Deserialize<Dictionary<string, double>>(payloadString);

        // Create the TemperatureRequest object
        var temperature = new ServerTemperatureData("temperature/server", new Dictionary<string, string>()
        {
            { "Термометр", data["id1"].ToString("0.00") }/*,
            { "Термометр в средата", data["id2"].ToString("0.00") },
            { "Термометр от долу", data["id3"].ToString("0.00") }*/
        }, DateTime.Now);
                
        
        if (temperature is null)
        {
            logger.Warning("{Topic} received empty or wrong data - {payload}","temperature/server",payloadString);
            return;
        }
        
        await mediator.Publish(new TemperatureDataReceivedEvent("temperature/server",temperature));
        
    }
}