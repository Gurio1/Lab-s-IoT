using IoT.Identity.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IoT.Identity;

public static class IdentityModuleExtension
{
    public static IServiceCollection AddIdentityModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<System.Reflection.Assembly> mediatRAssemblies)
    {

        services.Configure<ConfigurationAD>(
            config.GetSection(ConfigurationAD.Position));
        services.AddScoped<IAuthenticator, LdapAuthentication>();
        
        mediatRAssemblies.Add(typeof(IdentityModuleExtension).Assembly);
        
        logger.Information("{Module} module services registered", "Identity");

        return services;
    }
}