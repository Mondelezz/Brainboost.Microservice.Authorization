using Application.Abstraction.TokenAbstraction;
using System.Security.Claims;

namespace Application.TokenFeatures.TokenService;

public class TokenService : ITokenService
{
    public Task<ClaimsPrincipal> ValidateTokenAsync(string issuer, IList<string> audiences, string token) => throw new NotImplementedException();
}
