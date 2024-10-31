using Application.Abstraction.UserAbstraction;
using Application.Options;
using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.Client;
using FS.Keycloak.RestApiClient.Authentication.ClientFactory;
using FS.Keycloak.RestApiClient.Authentication.Flow;
using FS.Keycloak.RestApiClient.ClientFactory;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.Extensions.Configuration;

namespace Application.UserFeatures.UserService;

public class UserService(IConfiguration configuration) : IUserService
{
    /// <summary>
    /// The method gets all users from the specified realm.
    /// </summary>
    /// <param name="nameRealm">Name realm.</param>
    /// <returns>List of users.</returns>
    public async Task<IList<UserRepresentation>> GetUsersFromRealmAsync(string nameRealm)
    {
        using var usersApi = CreateUsersApi(nameRealm);

        IList<UserRepresentation> users =  await usersApi.GetUsersAsync(nameRealm);

        return users;
    }

    private UsersApi CreateUsersApi(string nameRealm)
    {
        PasswordGrantFlow credentials = GetPasswordGrantFlowCredentials(nameRealm);
        AuthenticationHttpClient httpClient = AuthenticationHttpClientFactory.Create(credentials);

        return ApiClientFactory.Create<UsersApi>(httpClient);
    }

    private PasswordGrantFlow GetPasswordGrantFlowCredentials(string nameRealm)
    {
        PasswordGrantFlowOptions passwordOptions = configuration
            .GetSection(nameof(PasswordGrantFlowOptions))
            .Get<PasswordGrantFlowOptions>()!;

        return new PasswordGrantFlow()
        {
            KeycloakUrl = passwordOptions.KeycloakUrl,
            Realm = nameRealm,
            UserName = passwordOptions.UserName,
            Password = passwordOptions.Password
        };
    }
}
