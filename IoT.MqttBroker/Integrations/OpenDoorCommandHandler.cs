using IoT.MqttBroker.Contracts;
using MediatR;

namespace IoT.MqttBroker.Integrations;

internal class OpenDoorCommandHandler : IRequestHandler<OpenDoorCommand>
{
    private readonly MqttBroker _broker;

    public OpenDoorCommandHandler(MqttBroker broker)
    {
        _broker = broker;
    }
    public async Task Handle(OpenDoorCommand request, CancellationToken cancellationToken)
    {
        await _broker.PublishMessage("Door", "Open");
    }
}