using System.Security.Claims;

namespace Application.Abstraction.TokenAbstraction;

public interface ITokenService
{
    public Task<ClaimsPrincipal> ValidateTokenAsync(string issuer, IList<string> audiences, string token);
}
