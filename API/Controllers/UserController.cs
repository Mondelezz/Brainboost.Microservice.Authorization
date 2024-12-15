using Application.Abstraction.UserAbstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Контроллер для работы с пользователями.
/// </summary>
[Route("api/v1/users")]
[ApiController]
public class UserController(IUserService userService) : Controller
{

    /// <summary>
    /// Получает информацию своего профиля.
    /// </summary>
    /// <returns>Инофрмация пользователя.</returns>
    [Authorize]
    [HttpGet("get-my-profile")]
    public ActionResult GetMyProfile()
    {
        ClaimsPrincipal claimsPrincipal = HttpContext.User;

        if (claimsPrincipal is null)
        {
            return NotFound();
        }

        return Ok(userService.GetMyProfile(claimsPrincipal));
    }
}
