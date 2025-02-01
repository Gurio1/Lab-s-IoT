namespace IoT.MqttBroker;

internal class ClientsCredentialOptions
{
    public const string Position = "MQTT Clients Credentials";

    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}