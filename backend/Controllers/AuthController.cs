using Microsoft.AspNetCore.Mvc;
using TaskManager.DTOs.Auth;
using TaskManager.Services;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto, HttpContext);

        if (!string.IsNullOrEmpty(result.Error))
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

        if (!string.IsNullOrEmpty(result.Error))
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        var token = await _authService._context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            await _authService._context.SaveChangesAsync();
        }

        return Ok(new { Message = "Logged out successfully" });
    }
}