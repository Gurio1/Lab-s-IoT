using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using HealthChecks.UI.Client;
using IoT.HealthCheck;
using IoT.Identity;
using IoT.Monitoring;
using IoT.MqttBroker;
using IoT.Temperature;
using IoT.Temperature.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MQTTnet.AspNetCore;
using Serilog;
//TODO : I dont know what to do with this little "features"(door,temp,monitoring).Too little for separate module.I'll let it be how it is rn,but after review it again

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel((context, o) =>
{
    o.ListenLocalhost(5089); // default http pipeline 
    o.ListenLocalhost(1883, l=>l.UseMqtt()); // default http pipeline 
    o.ListenLocalhost(8883, l =>
    {
        l.UseHttps(opt =>
        {
            var certificate = new X509Certificate2("C:/Users/Andrey-PC/Desktop/LAB/test.pfx","localhost", X509KeyStorageFlags.Exportable);

            opt.ServerCertificate = certificate;
        });
        l.UseMqtt();
    });
    o.ListenLocalhost(7219, l => l.UseHttps());
});

//TODO: Read how it will work with nginx.Especially tls cert.
// Configure Kestrel for MQTT on 1883 and HTTP on 5000
/*builder.WebHost.ConfigureKestrel((context, o) =>
{
    o.Listen(IPAddress.Parse("192.168.105.107"),5089); // default http pipeline 
    o.Listen(IPAddress.Parse("192.168.105.107"),1883, l=>l.UseMqtt()); // default http pipeline 
    o.Listen(IPAddress.Parse("192.168.105.107"),8883, l =>
    {
        l.UseHttps(opt =>
        {
            var certificate = new X509Certificate2("C:/Users/Andrey-PC/Desktop/LAB/test.pfx","localhost", X509KeyStorageFlags.Exportable);

            opt.ServerCertificate = certificate;
        });
        l.UseMqtt();
    });
    o.Listen(IPAddress.Parse("192.168.105.107"),7219, l => l.UseHttps());
});*/

builder.Host.UseSerilog((_, config) => 
    config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "localhost",
            ValidAudience = "localhost",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth:JwtSecret"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (!context.Request.Path.StartsWithSegments("/hub"))
                    return Task.CompletedTask;
                
                
                if (context.Request.Path.StartsWithSegments("/hub", StringComparison.OrdinalIgnoreCase) &&
                    context.Request.Query.TryGetValue("access_token", out var accessToken))
                {
                    context.Request.Headers.Append("Authorization", $"Bearer {accessToken}");
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

List<Assembly> mediatRAssemblies = [typeof(Program).Assembly];
builder.Services.AddIdentityModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddMqttBrokerServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddMonitoringModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddTemperatureModuleServices(builder.Configuration, logger, mediatRAssemblies);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(mediatRAssemblies.ToArray()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://192.168.105.107:4200", "https://localhost:4200","http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.AddHealthChecks()
    .AddCheck<IoTConnectionsHealthCheck>("iot-connections", HealthStatus.Unhealthy);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication()
    .UseAuthorization();

app.UseMqttBroker(app.Services,logger);


app.UseFastEndpoints()
    .UseSwaggerGen();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapHealthChecks("/health",
    new HealthCheckOptions()
    {
        ResultStatusCodes = new Dictionary<HealthStatus, int> {{HealthStatus.Unhealthy,203},{HealthStatus.Healthy,200},{HealthStatus.Degraded,203}},
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).RequireAuthorization();

app.MapHub<TemperatureHub>("/hub/temperature");

app.Run();
