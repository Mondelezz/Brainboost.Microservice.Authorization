using Application.Abstraction.UserAbstraction;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы с пользователями.
/// </summary>
[Route("user")]
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
    /// <param name="currentUserId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("{currentUserId}")]
    public async Task<ActionResult<UserRepresentation>> GetUserByIdAsync(string nameRealm, string currentUserId)
    {
        UserRepresentation user = await userService.GetUserFromRealmByIdAsync(nameRealm, currentUserId);

        return Ok(user);
    }

    /// <summary>
    /// Получает информацию своего профиля.
    /// </summary>
    /// <returns>Инофрмация пользователя.</returns>
    [Authorize]
    [HttpGet("get-my-profile")]
    public ActionResult<UserRepresentation> GetCurrentUserClaims()
    {
        ClaimsPrincipal claimsPrincipal = HttpContext.User;

        if (claimsPrincipal is null)
        {
            return NotFound();
        }

        List<string> claimTypes = new(
        [
            ClaimTypes.Email,
            ClaimTypes.NameIdentifier,
            ClaimTypes.Role,
            ClaimTypes.GivenName,
            ClaimTypes.Surname,
            "preferred_username"
        ]);

        Dictionary<string, string> claimsDictionary = claimTypes
            .Select(type => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == type))
            .Where(c => c != null)
            .ToDictionary(c => c!.Type, c => c!.Value);

        return Ok(claimsDictionary);
    }
}
