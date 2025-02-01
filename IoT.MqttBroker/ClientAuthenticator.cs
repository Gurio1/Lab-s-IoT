using Microsoft.Extensions.Options;

namespace IoT.MqttBroker;

internal class ClientAuthenticator
{
    private readonly ClientsCredentialOptions _clientCredentials;

    public ClientAuthenticator(IOptionsSnapshot<ClientsCredentialOptions> clientCredentials)
    {
        _clientCredentials = clientCredentials.Value;
    }

    public bool Authenticate(string userName,string password)
    {
        return (userName == _clientCredentials.UserName && password == _clientCredentials.Password);
    }
}