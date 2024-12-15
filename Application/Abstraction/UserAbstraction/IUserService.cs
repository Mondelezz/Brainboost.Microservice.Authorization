using FS.Keycloak.RestApiClient.Model;
using System.Security.Claims;

namespace Application.Abstraction.UserAbstraction;

public interface IUserService
{
    /// <summary>
    /// Получает список пользователей из указанной области 
    /// </summary>
    /// <param name="nameRealm">Наименование области.</param>
    /// <returns>Список пользователей.</returns>
    public Task<IList<UserRepresentation>> GetUsersFromRealmAsync(string nameRealm);

    /// <summary>
    /// Получает пользователя по идентификатору из указанной области
    /// </summary>
    /// <param name="nameRealm">Наименование области.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Пользователь</returns>
    public Task<UserRepresentation> GetUserFromRealmByIdAsync(string nameRealm, string userId);

    /// <summary>
    /// Получает информацию профиля текущего пользователя
    /// </summary>
    /// <param name="claimsPrincipal">Пользователь</param>
    /// <returns>Словарь с данными пользователя</returns>
    public Dictionary<string, string> GetMyProfile(ClaimsPrincipal claimsPrincipal);
}
