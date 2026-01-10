using Application.Extensions;
using AutoMapper;
using Consts;
using Domain;
using Domain.Abstraction;
using Domain.Abstraction.Authentication;
using Domain.Abstraction.Base;
using Domain.Abstraction.Errors;
using Domain.Abstraction.Jwt;
using Domain.Abstraction.Results;
using Domain.EfClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Service;

public class AuthService : JwtTokenService, IAuthService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIpGeolocationService _ipGeolocationService;
    private readonly IMapper _mapper;

    public AuthService(
        ITokenRepository tokenRepository,
        IOptions<JwtOption> jwtOptions,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork,
        IIpGeolocationService ipGeolocationService,
        IMapper mapper)
        : base(jwtOptions, httpContextAccessor, userRepository, tokenRepository)
    {
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _ipGeolocationService = ipGeolocationService;
        _mapper = mapper;
    }

    public async Task<Result> DeleteAsync(Guid accessTokenId)
    {
        var tokenResult = await _tokenRepository.GetByAccessTokenId(accessTokenId);

        if (tokenResult == null)
            return Result.Failure(Error.TokenNotFound);

        _tokenRepository.Delete(tokenResult.Value);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<TokenDto>> GenerateTokenAsync(GenerateTokenDto dto)
    {
        var httpContext = HttpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty;
        var ipAddress = httpContext?.GetClientIpAddress() ?? string.Empty;

        var aclaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, dto.User.Id.ToString()),
            new(ClaimTypes.Name, dto.User.UserName),
            new("IpAddress", ipAddress),
            new("AccessTokenId", dto.AccessTokenId.ToString())
        };

        var rclaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, dto.User.Id.ToString()),
            new(ClaimTypes.Name, dto.User.UserName),
            new("IpAddress", ipAddress),
            new("RefreshTokenId", dto.RefreshTokenId.ToString())
        };

        var (accessToken, atokenOptions) = GenerateAccessToken(dto.User, aclaims);
        var (refreshToken, rtokenOptions) = GenerateRefreshToken(dto.User, rclaims);

        if (dto.IsUpdateToken)
        {
            return await HandleTokenUpdate(
                dto.User,
                dto.AccessTokenId,
                dto.RefreshTokenId,
                accessToken,
                atokenOptions,
                refreshToken,
                rtokenOptions,
                AuthTypeConst.USERNAME);
        }

        return await HandleNewTokenCreation(
            dto.User,
            dto.AccessTokenId,
            dto.RefreshTokenId,
            accessToken,
            atokenOptions,
            refreshToken,
            rtokenOptions,
            userAgent,
            ipAddress,
            AuthTypeConst.USERNAME);
    }

    public async Task<Result<TokenDto>> LoginAsync(LoginRequest loginRequest)
    {
        var userResult = await _userRepository.GetByUsername(loginRequest.Username);

        if (!userResult.IsSuccess || userResult.Value == null)
            return Result<TokenDto>.Failure(Error.LoginFaild);

        var result = _passwordHasher.VerifyHashedPassword(
            new User(),
            userResult.Value.PasswordHash!,
            loginRequest.Password);

        if (result == PasswordVerificationResult.Failed)
            return Result<TokenDto>.Failure(Error.LoginFaild);

        var generateTokenDto = new GenerateTokenDto
        {
            User = userResult.Value,
            IsUpdateToken = false,
            AccessTokenId = Guid.NewGuid(),
            RefreshTokenId = Guid.NewGuid()
        };

        var token = await GenerateTokenAsync(generateTokenDto);

        if (!token.IsSuccess)
            return Result<TokenDto>.Failure(token.Error);

        var response = new TokenDto
        {
            AccessToken = token.Value!.AccessToken,
            AccessTokenExpiryTime = token.Value.AccessTokenExpiryTime,
            RefreshToken = token.Value.RefreshToken,
            RefreshTokenExpiryTime = token.Value.RefreshTokenExpiryTime,
            AuthType = token.Value.AuthType
        };

        return Result<TokenDto>.Success(response);
    }

    public async Task<Result<TokenDto>> LoginWithGoogleAsync(string token)
    {
        throw new NotImplementedException("Google authentication not implemented yet");
    }

    public async Task<Result> LogoutAsync()
    {
        return await DeleteAsync(AccessTokenId!.Value);
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken)
    {
        var tokenResult = await _tokenRepository.GetByAccessTokenId(AccessTokenId!.Value);

        if (tokenResult == null)
            return Result<TokenDto>.Failure(Error.TokenNotFound);

        var isValidRefreshToken = await ValidateRefreshToken(refreshToken);

        if (!isValidRefreshToken)
        {
            return Result<TokenDto>.Failure(Error.RefreshTokenInvalid);
        }

        var userResult = await _userRepository.GetByIdAsync(UserId!.Value);

        if (userResult == null)
            return Result<TokenDto>.Failure(Error.UserNotFound);

        var generateTokenDto = new GenerateTokenDto
        {
            User = userResult,
            IsUpdateToken = true,
            AccessTokenId = Guid.NewGuid(),
            RefreshTokenId = Guid.NewGuid()
        };

        var newToken = await GenerateTokenAsync(generateTokenDto);

        if (!newToken.IsSuccess)
            return Result<TokenDto>.Failure(newToken.Error);

        var response = new TokenDto
        {
            AccessToken = newToken.Value!.AccessToken,
            AccessTokenExpiryTime = newToken.Value.AccessTokenExpiryTime,
            RefreshToken = newToken.Value.RefreshToken,
            RefreshTokenExpiryTime = newToken.Value.RefreshTokenExpiryTime,
            AuthType = newToken.Value.AuthType
        };

        return Result<TokenDto>.Success(response);
    }

    #region Private Methods

    private async Task<Result<TokenDto>> HandleTokenUpdate(
        User user,
        Guid accessTokenId,
        Guid refreshTokenId,
        string accessToken,
        JwtSecurityToken accessTokenOptions,
        string refreshToken,
        JwtSecurityToken refreshTokenOptions,
        int authType)
    {
        var existingTokenResult = await _tokenRepository.FindByUserId(user.Id);

        if (existingTokenResult.IsSuccess && existingTokenResult.Value != null)
        {
            existingTokenResult.Value.UpdateAccessToken(accessTokenId, accessTokenOptions.ValidTo);
            existingTokenResult.Value.UpdateRefreshToken(refreshTokenId, refreshTokenOptions.ValidTo);

            if (existingTokenResult.Value.DeviceInfo != null)
            {
                existingTokenResult.Value.DeviceInfo.UpdateActivity();
            }

            await _unitOfWork.SaveChangesAsync();

            var result = new TokenDto
            {
                AccessToken = accessToken,
                AccessTokenExpiryTime = accessTokenOptions.ValidTo,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenOptions.ValidTo,
                AuthType = authType.ToString()
            };

            return Result<TokenDto>.Success(result);
        }

        return Result<TokenDto>.Failure(Error.TokenNotFound);
    }

    private (string AccessToken, JwtSecurityToken TokenOptions) GenerateAccessToken(User user, List<Claim> claims)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey!));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes ?? 60);

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer!,
            audience: _jwtOptions.Audience!,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(tokenOptions);

        return (accessToken, tokenOptions);
    }

    private (string RefreshToken, JwtSecurityToken TokenOptions) GenerateRefreshToken(User user, List<Claim> claims)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.RefreshTokenSecretKey!));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiresInDays ?? 30);

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer!,
            audience: _jwtOptions.Audience!,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var refreshToken = tokenHandler.WriteToken(tokenOptions);

        return (refreshToken, tokenOptions);
    }

    private async Task<Result<TokenDto>> HandleNewTokenCreation(
        User user,
        Guid accessTokenId,
        Guid refreshTokenId,
        string accessToken,
        JwtSecurityToken accessTokenOptions,
        string refreshToken,
        JwtSecurityToken refreshTokenOptions,
        string userAgent,
        string ipAddress,
        int authType)
    {
        // 1. Device ma'lumotlarini olish
        var deviceInfo = DeviceInfoExtractor.GetDetailedDeviceInfo(userAgent);

        // 2. IP geolocation ma'lumotlarini olish (async)
        IpLocationInfo? locationInfo = null;
        try
        {
            locationInfo = await _ipGeolocationService.GetLocationInfoAsync(ipAddress);
        }
        catch// (Exception ex)
        {
            // Log the error but don't fail the login process
            // _logger.LogWarning(ex, "Failed to get IP geolocation for {IpAddress}", ipAddress);
        }

        // 3. Token entity yaratish
        var tokenEntity = new Token
        {
            UserId = user.Id,
            AccessTokenId = accessTokenId,
            AccessTokenExpireAt = accessTokenOptions.ValidTo,
            RefreshTokenId = refreshTokenId,
            RefreshTokenExpireAt = refreshTokenOptions.ValidTo
        };

        // 4. DeviceInfo entity yaratish (AutoMapper orqali)
        var deviceInfoEntity = _mapper.Map<DeviceInfo>(deviceInfo);
        deviceInfoEntity.TokenId = tokenEntity.Id;
        deviceInfoEntity.IpAddress = ipAddress;
        deviceInfoEntity.UserAgent = userAgent;

        // 5. Geolocation ma'lumotlarini qo'shish
        if (locationInfo != null)
        {
            deviceInfoEntity.UpdateGeolocation(locationInfo);
        }

        // 6. Token va DeviceInfo ni saqlash
        tokenEntity.DeviceInfo = deviceInfoEntity;
        await _tokenRepository.AddAsync(tokenEntity);
        await _unitOfWork.SaveChangesAsync();

        // 7. Response yaratish
        var result = new TokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiryTime = accessTokenOptions.ValidTo,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = refreshTokenOptions.ValidTo,
            AuthType = authType.ToString()
        };

        return Result<TokenDto>.Success(result);
    }

    #endregion
}