using Application.Abstraction.UserAbstraction;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class AdministratorController(IUserService userService) : Controller
{
    /// <summary>
    /// Get users from realm
    /// </summary>
    /// <param name="nameRealm">name realm</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("get-users-from-realm")]
    public async Task<ActionResult<IReadOnlyList<UserRepresentation>>> GetUsersFromRealm(string nameRealm)
    {
        IList<UserRepresentation> users = await userService.GetUsersFromRealmAsync(nameRealm);

        return Ok(users);
    }
}
