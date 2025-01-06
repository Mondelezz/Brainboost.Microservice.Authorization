using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using API.Options;
using Application.Abstraction.UserAbstraction;

namespace API.Controllers;

/// <summary>
/// Контроллер отвечает за управление токенами доступа.
/// </summary>
[Route("api/v1/token")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly JsonWebKeyOptions _jwkOpt;
    private readonly AuthenticationOptions _authOpt;
    private readonly ILogger<TokenController> _logger;
    private readonly IUserService _userService;

    public TokenController(
        IOptionsSnapshot<JsonWebKeyOptions> jwkOpt,
        IOptionsSnapshot<AuthenticationOptions> authOpt,
        ILogger<TokenController> logger,
        IUserService userService)
    {
        _jwkOpt = jwkOpt.Value;
        _authOpt = authOpt.Value;
        _logger = logger;
        _userService = userService;
    }

    [HttpPost("validate-jwt-token-keycloak")]
    public ActionResult<Dictionary<string, string>> ValidateJwtTokenKeycloak(string authToken)
    {
        if (string.IsNullOrEmpty(authToken))
        {
            _logger.LogWarning("Token is missing.");
            return BadRequest("Token is missing.");
        }

        string issuer = _authOpt.ValidIssuer;
        IList<string> audiences = _authOpt.Audience;

        JwtSecurityTokenHandler tokenHandler = new();
        TokenValidationParameters validationParameters = new()
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateActor = true,
            ValidateIssuerSigningKey = true,
            ValidAudiences = audiences,
            ValidIssuer = issuer,
            IssuerSigningKey = new JsonWebKey()
            {
                Kid = _jwkOpt.Kid,
                Kty = _jwkOpt.Kty,
                Alg = _jwkOpt.Alg,
                Use = _jwkOpt.Use,
                N = _jwkOpt.N,
                E = _jwkOpt.E,
                X5t = _jwkOpt.X5t,
                X5tS256 = _jwkOpt.X5tS256
            }
        };

        ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken securityToken);

        if (claimsPrincipal == null || securityToken == null)
        {
            _logger.LogError("Error validating token.");
            throw new UnauthorizedAccessException();
        }

        return _userService.GetMyProfile(claimsPrincipal);
    }
}
