using Domain;
using Domain.Abstraction.Authentication;
using Domain.Abstraction.Results;
using Domain.EfClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.WebApi.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ProjectBase.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("SignIn")]
    [SwaggerOperation(
        Summary = "Foydalanuvchi tizimga kirishi",
        Description = "Foydalanuvchi tizimga login va parol orqali kirishi."
    )]
    [SwaggerResponse(200, "Muvaffaqiyatli login", typeof(Result<TokenDto>))]
    [SwaggerResponse(400, "Login yoki parol noto'g'ri")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenDto>> Login([Required][FromBody] LoginRequest loginRequest)
    {
        return await _authService.LoginAsync(loginRequest).ToActionResultSafeAsync(_logger);
    }

    [HttpGet("RefreshToken")]
    [SwaggerOperation(
        Summary = "Access tokenni yangilash",
        Description = "Amaldagi access token va refresh token orqali yangi access token yaratish."
    )]
    [SwaggerResponse(200, "Tokens muvaffaqiyatli yangilandi", typeof(Result<TokenDto>))]
    [SwaggerResponse(400, "Noto'g'ri so'rov ma'lumotlari")]
    [SwaggerResponse(401, "Avtorizatsiya talab qilinadi")]
    [SwaggerResponse(500, "Server ichki xatosi")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenDto>> RefreshToken([Required] string refreshToken)
    {
        return await _authService.RefreshTokenAsync(refreshToken).ToActionResultSafeAsync(_logger);
    }

    [HttpGet("Logout")]
    [SwaggerOperation(
        Summary = "Foydalanuvchini tizimdan chiqish",
        Description = "Foydalanuvchini tizimdan muvaffaqiyatli chiqishini ta'minlaydi."
    )]
    [SwaggerResponse(200, "Foydalanuvchi muvaffaqiyatli chiqdi", typeof(Result))]
    [SwaggerResponse(401, "Avtorizatsiya talab qilinadi")]
    [SwaggerResponse(500, "Xato yuz berdi, chiqishda muammo")]
    public async Task<ActionResult> UserLogout()
    {
        return await _authService.LogoutAsync().ToActionResultSafeAsync(_logger);
    }

    [HttpGet("IsSecure")]
    [SwaggerOperation(
        Summary = "Foydalanuvchining xavfsizligini tekshirish",
        Description = "Foydalanuvchi avtorizatsiya qilinganligi va xavfsizligini tasdiqlaydi."
    )]
    [SwaggerResponse(200, "Foydalanuvchi muvaffaqiyatli avtorizatsiya qilingan", typeof(Result))]
    [SwaggerResponse(401, "Avtorizatsiya talab qilinadi")]
    public ActionResult<Result> IsSecureUser()
    {
        return Result.Success().ToActionResult();
    }
}