using System.Text;
using IoT.MqttBroker.Contracts;
using MediatR;
using MQTTnet.Server;

namespace IoT.MqttBroker.Integrations;

internal class GetLastReceivedMessageHandler : IRequestHandler<GetLastReceivedMessageQuery,string>
{
    private readonly MqttServer _server;

    public GetLastReceivedMessageHandler(MqttServer server)
    {
        _server = server;
    }

    public async Task<string> Handle(GetLastReceivedMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await _server.GetRetainedMessageAsync(request.Topic);

        return message is null ? "Data still not received" : Encoding.UTF8.GetString(message.PayloadSegment);
    }
}