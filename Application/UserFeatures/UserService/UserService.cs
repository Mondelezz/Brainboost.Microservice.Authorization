using Application.Abstraction.UserAbstraction;
using Application.Common.Exceptions;
using Application.Options;
using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.Client;
using FS.Keycloak.RestApiClient.Authentication.ClientFactory;
using FS.Keycloak.RestApiClient.Authentication.Flow;
using FS.Keycloak.RestApiClient.ClientFactory;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;


namespace Application.UserFeatures.UserService;

public class UserService(IConfiguration configuration) : IUserService
{
    /// <summary>
    /// Получает пользователя по идентификатору из указанной области
    /// </summary>
    /// <param name="nameRealm">Наименование области.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Пользователь</returns>
    public async Task<UserRepresentation> GetUserFromRealmByIdAsync(string nameRealm, string userId)
    {
        using UsersApi usersApi = CreateUsersApi(nameRealm);

        UserRepresentation user = await usersApi.GetUsersByUserIdAsync(nameRealm, userId)
            ?? throw new EntityNotFoundException(userId, "User");

        return user;
    }

    /// <summary>
    /// Получает список пользователей из указанной области 
    /// </summary>
    /// <param name="nameRealm">Наименование области.</param>
    /// <returns>Список пользователей.</returns>
    public async Task<IList<UserRepresentation>> GetUsersFromRealmAsync(string nameRealm)
    {
        using UsersApi usersApi = CreateUsersApi(nameRealm);

        IList<UserRepresentation> users = await usersApi.GetUsersAsync(nameRealm);

        return users;
    }

    /// <summary>
    /// Получает информацию профиля текущего пользователя
    /// </summary>
    /// <param name="claimsPrincipal">Пользователь</param>
    /// <returns>Словарь с данными пользователя</returns>
    public Dictionary<string, string> GetMyProfile(ClaimsPrincipal claimsPrincipal)
    {
        List<string> claimTypes = new([ClaimTypes.Email, ClaimTypes.NameIdentifier, ClaimTypes.Role, ClaimTypes.GivenName, ClaimTypes.Surname, "preferred_username"]);

        return claimTypes
            .Select(type => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == type))
            .Where(c => c != null)
            .ToDictionary(c => c!.Type, c => c!.Value);
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
