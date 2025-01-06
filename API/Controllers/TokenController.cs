using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using API.Options;

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
    public TokenController(IOptions<JsonWebKeyOptions> jwkOpt, IOptions<AuthenticationOptions> authOpt)
    {
        _jwkOpt = jwkOpt.Value;
        _authOpt = authOpt.Value;
    }
    [HttpPost("validate-token")]
    public ActionResult<Dictionary<string, string>> ValidateToken(string authToken)
    {
        if (string.IsNullOrEmpty(authToken))
        {
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
                E = _jwkOpt.N,
                X5t = _jwkOpt.X5t,
                X5tS256 = _jwkOpt.X5tS256
            }
        };

        ClaimsPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken securityToken);

        if (principal == null || securityToken == null)
        {
            throw new UnauthorizedAccessException();
        }

        List<string> claimTypes = new(
            [
                ClaimTypes.Email,
                ClaimTypes.NameIdentifier,
                ClaimTypes.Role, ClaimTypes.GivenName,
                ClaimTypes.Surname,
                "preferred_username"
            ]);

        return claimTypes
            .Select(type => principal.Claims.FirstOrDefault(c => c.Type == type))
            .Where(c => c != null)
            .ToDictionary(c => c!.Type, c => c!.Value);
    }
}
