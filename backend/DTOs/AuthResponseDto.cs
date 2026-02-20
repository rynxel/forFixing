namespace TaskManager.DTOs.Auth;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    string Message,
    string? Error = null
);