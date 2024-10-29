using API.Abstraction.Users;
using Application.UserFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class AdministratorController(IUserManagement management) : Controller
{
    /// <summary>
    /// Get users from realm
    /// </summary>
    /// <param name="baseUrl">url address keycloak</param>
    /// <param name="nameRealm">name realm</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("get-users-from-realm")]
    public async Task<ActionResult<IReadOnlyList<UserFromRealmGetDto>>> GetUsersFromRealm(
        string baseUrl,
        string nameRealm)
    {
        IReadOnlyList<UserFromRealmGetDto> users = await management.GetUsersFromRealm(
            baseUrl,
            nameRealm);

        return Ok(users);
    }
}
