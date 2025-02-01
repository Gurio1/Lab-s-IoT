using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Extensions.Options;
using Serilog;

namespace IoT.Identity.Authentication;

internal class LdapAuthentication(IOptions<ConfigurationAD> options,ILogger logger) : IAuthenticator
{
    private readonly string _ldapServer = options.Value.Domain;
    private readonly string _ldapBaseDn = options.Value.BaseDn;
    private readonly string _ou = options.Value.OU;

    public bool Authenticate(string username, string password)
    {
        try
        {
            using (var ldapConnection = new LdapConnection(_ldapServer))
            {
                ldapConnection.AuthType = AuthType.Basic;
                ldapConnection.Credential = new NetworkCredential($"{username}@{_ldapServer}", password);
                ldapConnection.SessionOptions.ProtocolVersion = 3;

                // Attempt to bind to force authentication
                ldapConnection.Bind();

                // Log successful authentication
                logger.Information("Authentication successful.");

                // Perform a search for the user's display name at the domain level
                var searchRequest = new SearchRequest($"ou={_ou}, {_ldapBaseDn}", $"(sAMAccountName={username})", SearchScope.Subtree, "displayName");
                var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                if (searchResponse.Entries.Count > 0)
                {
                    //TODO: Take object id
                    var displayName = searchResponse.Entries[0].Attributes["displayName"][0].ToString();
                    //return displayName;
                    return true;
                }
                else
                {
                    logger.Information($"User '{username}' not found.");
                }
            }
        }
        catch (LdapException ex)
        {
            logger.Error($"LDAP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.Error($"An error occurred: {ex.Message}");
        }

        return false; // Return null if authentication or display name retrieval failed
    }
}