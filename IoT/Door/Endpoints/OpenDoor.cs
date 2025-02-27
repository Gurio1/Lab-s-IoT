using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace IoT.Door.Endpoints;

internal class OpenDoor : EndpointWithoutRequest<string>
{
    
    public override void Configure()
    {
        Post("api/door/open");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync("test", cancellation: ct);
    }
}