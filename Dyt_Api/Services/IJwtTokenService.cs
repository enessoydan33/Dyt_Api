using Dyt_Api.Entities;

namespace Dyt_Api.Services
{
    public interface IJwtTokenService
    {
        Task<JwtTokenResult> GenerateTokenAsync(ApplicationUser user);
    }

    public sealed record JwtTokenResult(string Token, DateTime Expiration);
}
