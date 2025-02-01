namespace IoT.Identity.Authentication;

//TODO: Find way of implementation
internal interface IAuthenticator
{
    public bool Authenticate(string username, string password);
}