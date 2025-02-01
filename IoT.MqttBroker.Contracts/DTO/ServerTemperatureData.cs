namespace IoT.MqttBroker.Contracts.DTO;

public record ServerTemperatureData(string TopicName,Dictionary<string,string> ThermometersData,DateTime DateTime);