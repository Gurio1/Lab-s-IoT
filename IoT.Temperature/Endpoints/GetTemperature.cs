using FastEndpoints;

namespace IoT.Temperature.Endpoints;

internal class GetTemperature : Endpoint<GetTemperatureRequest>
{
    private readonly TemperatureService _temperatureService;

    public GetTemperature(TemperatureService temperatureService)
    {
        _temperatureService = temperatureService;
    }
    public override void Configure()
    {
        Post("/temperature");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTemperatureRequest req, CancellationToken ct)
    {
        var records = await _temperatureService.GetLastRecordsAsync(req.Count);

        await SendAsync(records, cancellation: ct);
    }
}