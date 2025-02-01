using IoT.MqttBroker.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IoT.Temperature.Hubs;

[Authorize]
public class TemperatureHub : Hub
{
    private readonly IMediator _mediator;

    public TemperatureHub(IMediator mediator)
    {
        _mediator = mediator;
    }
    /*public override async Task OnConnectedAsync()
    {
        var message = await _mediator.Send(new GetLastReceivedMessageQuery("temperature/server"));

        await Clients.Caller.SendAsync("ReceiveTemperature",message);
    }*/

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveTemperature", message);
    }
}