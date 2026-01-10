using Domain.Abstraction.Authentication;
using Domain.Abstraction.Results;
using Domain.EfClasses;

namespace Domain;

public interface IAuthService
{
    Task<Result<TokenDto>> GenerateTokenAsync(GenerateTokenDto dto);
    Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken);
    Task<Result<TokenDto>> LoginAsync(LoginRequest loginRequest);
    Task<Result<TokenDto>> LoginWithGoogleAsync(string token);
    Task<Result> LogoutAsync();
    Task<Result> DeleteAsync(Guid accessTokenId);
    bool IsAuthenticated { get; }
}