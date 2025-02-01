using MQTTnet.AspNetCore.Routing;
using Serilog;

namespace IoT.MqttBroker.Controllers;

internal class DoorController(ILogger logger,MqttBroker broker) : MqttBaseController
{
    public async Task OpenDoor()
    {
        await broker.PublishMessage("DOOR", "Open");
    }

    [MqttRoute("door/status")]
    public Task ReceiveDeviceStatus()
    {
        //TODO: Implement IoT status handler
        return Task.CompletedTask;
    }
}