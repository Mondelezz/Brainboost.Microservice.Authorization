using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Контроллер отвечает за управление токенами доступа.
/// </summary>
[Route("api/v1/token")]
[ApiController]
public class TokenController : ControllerBase
{
}
