using IoT.MqttBroker.Contracts.DTO;
using MediatR;

namespace IoT.MqttBroker.Contracts;

public record TemperatureDataReceivedEvent(string Topic, ServerTemperatureData Data) : INotification;