using FastEndpoints;
using FastEndpoints.Security;
using IoT.Identity.Authentication;

namespace IoT.Identity.Endpoints;

internal class Login(IAuthenticator authenticator) : Endpoint<LoginRequest>
{
    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        //var loginSuccessful = authenticator.Authenticate(req.Email, req.Password);

        var loginSuccessful = true;
        if (!loginSuccessful)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var jwtSecret = Config["Auth:JwtSecret"]!;
        var token = JwtBearer.CreateToken(opt =>
        {
            opt.SigningKey = jwtSecret;
            opt.Audience = "localhost";
            opt.Issuer = "localhost";
            opt.ExpireAt = DateTime.UtcNow.AddHours(4);
            /*opt.User["PlayerIdentityId"] = user.Id;
            opt.User["PlayerId"] = user.PlayerId.ToString();*/
        });

        await SendAsync(new {token}, cancellation: ct);
    }
}