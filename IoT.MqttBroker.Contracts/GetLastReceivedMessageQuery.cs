using MediatR;

namespace IoT.MqttBroker.Contracts;

public record GetLastReceivedMessageQuery(string Topic) : IRequest<string>;