using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TaskManager.Models;
using TaskManager.DTOs.Auth;

namespace TaskManager.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // Login user and generate tokens
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, HttpContext httpContext)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            await LogAttempt(null, "FAILED_LOGIN", httpContext);
            return new AuthResponseDto("", "", "Invalid email or password", "AUTH_FAILED");
        }

        if (!user.IsActive)
            return new AuthResponseDto("", "", "Account is inactive", "ACCOUNT_DISABLED");

        // Generate tokens
        var (accessToken, refreshToken) = await GenerateTokensAsync(user);

        // Log successful login
        await LogAttempt(user.Id, "LOGIN_SUCCESS", httpContext);

        return new AuthResponseDto(accessToken, refreshToken, "Login successful");
    }

    // Generate access + refresh tokens
    public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateJwtToken(user);

        // Refresh token: secure random string
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var userToken = new UserToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.UserTokens.Add(userToken);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    // Generate JWT access token
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Refresh access token using refresh token
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var tokenEntry = await _context.UserTokens
            .Include(t => t.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (tokenEntry == null || tokenEntry.IsRevoked || tokenEntry.ExpiresAt < DateTime.UtcNow)
            return new AuthResponseDto("", "", "Invalid or expired refresh token", "TOKEN_INVALID");

        // Optional: revoke old refresh token and issue new one (rotation)
        tokenEntry.IsRevoked = true;
        var (accessToken, newRefreshToken) = await GenerateTokensAsync(tokenEntry.User);

        return new AuthResponseDto(accessToken, newRefreshToken, "Token refreshed successfully");
    }

    // Async logging
    private async Task LogAttempt(int? userId, string action, HttpContext context)
    {
        var log = new ActivityLog
        {
            UserId = userId,
            Action = action,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers["User-Agent"]
        };

        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}