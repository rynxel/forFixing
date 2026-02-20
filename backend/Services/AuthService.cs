using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Models;
using TaskManager.DTOs.Auth;
using TaskManager.Data;
using BCrypt.Net;

namespace TaskManager.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    public AuthService(ApplicationDbContext context, IConfiguration config)
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

        // Generate access + refresh token
        var (accessToken, refreshToken) = await GenerateTokensAsync(user);

        // Log login
        await LogAttempt(user.Id, "LOGIN_SUCCESS", httpContext);

        return new AuthResponseDto(accessToken, refreshToken, "Login successful");
    }

    // Logout: revoke refresh token
    public async Task LogoutAsync(string refreshToken)
    {
        var token = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
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

        // Revoke old refresh token and issue new one
        tokenEntry.IsRevoked = true;
        var (accessToken, newRefreshToken) = await GenerateTokensAsync(tokenEntry.User);

        return new AuthResponseDto(accessToken, newRefreshToken, "Token refreshed successfully");
    }

    // Generate access + refresh tokens
    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateJwtToken(user);

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

    // Generate JWT
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

    // Log login/logout attempts
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