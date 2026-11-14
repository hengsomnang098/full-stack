namespace FullStack.Application.DTOs.Auth;

/// <summary>Tokens are delivered via HttpOnly cookies, never in the body.</summary>
public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public AuthUserDto User { get; set; } = new();
}
