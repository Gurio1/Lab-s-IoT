using IoT.MqttBroker.Contracts;
using IoT.Temperature.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Serilog;

namespace IoT.Temperature.Integrations;

public class TemperatureReceivedEventHandler(IHubContext<TemperatureHub> hubContext,TemperatureService temperatureService)
    : INotificationHandler<TemperatureDataReceivedEvent>
{
    public async Task Handle(TemperatureDataReceivedEvent notification, CancellationToken cancellationToken)
    {
        await temperatureService.CreateAsync(notification.Data);
        await hubContext.Clients.All.SendAsync("ReceiveTemperature",notification.Data, cancellationToken: cancellationToken);
    }
}