using IoT.MqttBroker.Contracts.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IoT.Temperature;

public class ServerThermometersReport
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string TopicName { get; set; }
    public required Dictionary<string,string> ThermometersData { get; set; }
    
    [BsonSerializer(typeof(LocalDateTimeSerializer))]
    public DateTime DateTime { get; set; }
    
    public static implicit operator ServerThermometersReport(ServerTemperatureData serverTemperatureData) => new()
    {
        TopicName = serverTemperatureData.TopicName,
        ThermometersData = serverTemperatureData.ThermometersData,
        DateTime = serverTemperatureData.DateTime
    };

}