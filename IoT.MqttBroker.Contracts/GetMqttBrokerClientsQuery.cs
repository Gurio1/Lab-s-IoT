using MediatR;
using MQTTnet.Server;

namespace IoT.MqttBroker.Contracts;

public record GetMqttBrokerClientsQuery() : IRequest<IList<MqttClientStatus>>;