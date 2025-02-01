using MongoDB.Bson.Serialization;

namespace IoT.Temperature;

//MongoDB cannot save time in local format
internal class LocalDateTimeSerializer : IBsonSerializer<DateTime>
{
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value is DateTime dateTime)
        {
            Serialize(context, args, dateTime);
        }
        else
        {
            throw new ArgumentException($"Expected value to be of type {typeof(DateTime)}, but got {value.GetType()}.");
        }
    }

    public Type ValueType => typeof(DateTime);

    public DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonDateTime = context.Reader.ReadDateTime();
        return DateTime.SpecifyKind(new DateTime(bsonDateTime), DateTimeKind.Utc).ToLocalTime();
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
    {
        var utcDateTime = value.ToUniversalTime();
        context.Writer.WriteDateTime(utcDateTime.Ticks);
    }
}