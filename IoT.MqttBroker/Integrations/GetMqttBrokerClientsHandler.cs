using IoT.MqttBroker.Contracts;
using MediatR;
using MQTTnet.Server;

namespace IoT.MqttBroker.Integrations;

internal class GetMqttBrokerClientsHandler : IRequestHandler<GetMqttBrokerClientsQuery,IList<MqttClientStatus>>
{
    private readonly MqttServer _server;

    public GetMqttBrokerClientsHandler(MqttServer server)
    {
        _server = server;
    }
    public async Task<IList<MqttClientStatus>> Handle(GetMqttBrokerClientsQuery request, CancellationToken cancellationToken)
    {
        return await _server.GetClientsAsync();
    }
}