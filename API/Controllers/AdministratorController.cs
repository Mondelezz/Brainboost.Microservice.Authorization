using Application.Abstraction.UserAbstraction;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы администратора.
/// </summary>
[Route("api/v1/admin")]
public class AdministratorController(IUserService userService) : Controller
{
    /// <summary>
    /// Получает список всех пользователей конкретной области.
    /// </summary>
    /// <param name="nameRealm">Наименование области.</param>
    /// <returns>Список пользователей.</returns>
    [Authorize]
    [HttpGet("get-users-from-realm")]
    public async Task<ActionResult<IReadOnlyList<UserRepresentation>>> GetUsersFromRealm(string nameRealm)
    {
        IList<UserRepresentation> users = await userService.GetUsersFromRealmAsync(nameRealm);

        return Ok(users);
    }

    /// <summary>
    /// Получает пользователя по идентификатору.
    /// </summary>
    /// <param name="nameRealm">Наименование области.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserRepresentation>> GetUserByIdAsync(string nameRealm, string userId)
    {
        UserRepresentation user = await userService.GetUserFromRealmByIdAsync(nameRealm, userId);

        return Ok(user);
    }
}
