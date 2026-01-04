using Domain;
using Domain.Abstraction.Authentication;
using Domain.Abstraction.Errors;
using Domain.Abstraction.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ProjectBase.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("SignIn")]
    [SwaggerOperation(
        Summary = "Foydalanuvchi tizimga kirishi",
        Description = "Foydalanuvchi tizimga login va parol orqali kirishi."
    )]
    [SwaggerResponse(200, "Muvaffaqiyatli login", typeof(Result<LoginRequest>))]
    [SwaggerResponse(400, "Login yoki parol noto'g'ri")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<LoginRequest>>> Login([Required][FromBody] LoginRequest loginRequest)
    {

        var userToken = await _authService.LoginAsync(loginRequest);
        if (userToken.IsFailure) return BadRequest(userToken);

        return Ok(userToken);
    }

    [HttpGet("RefreshToken")]
    [SwaggerOperation(
            Summary = "Access tokenni yangilash",
            Description = "Amaldagi access token va refresh token orqali yangi access token yaratish."
        )]
    [SwaggerResponse(200, "Tokens muvaffaqiyatli yangilandi", typeof(Result<LoginRequest>))]
    [SwaggerResponse(400, "Noto'g'ri so'rov ma'lumotlari")]
    [SwaggerResponse(401, "Avtorizatsiya talab qilinadi")]
    [SwaggerResponse(500, "Server ichki xatosi")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<LoginRequest>>> RefreshToken([Required] string refreshToken)
    {
        try
        {
            var newToken = await _authService.RefreshTokenAsync(refreshToken);

            if (newToken.IsFailure) return BadRequest(newToken);

            return Ok(newToken);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, Result<LoginRequest>.Failure(Error.NullValue));
        }
    }

    [HttpGet("Logout")]
    [SwaggerOperation(
            Summary = "Foydalanuvchini tizimdan chiqish",
            Description = "Foydalanuvchini tizimdan muvaffaqiyatli chiqishini ta'minlaydi."
        )]
    [SwaggerResponse(200, "Foydalanuvchi muvaffaqiyatli chiqdi", typeof(Result))]
    [SwaggerResponse(401, "Avtorizatsiya talab qilinadi")]
    [SwaggerResponse(500, "Xato yuz berdi, chiqishda muammo")]
    public async Task<ActionResult<Result>> UserLogout()
    {
        try
        {
            var result = await _authService.LogoutAsync();

            if (result.IsSuccess) return Ok(result);

            return BadRequest(Result.Failure(Error.LogoutFaild));

        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, Result.Failure(Error.NullValue));
        }
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
        return Ok(Result.Success());
    }
}