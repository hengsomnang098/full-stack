namespace FullStack.Application.DTOs.Auth;

public class LoginRequest
{
    /// <summary>Employee telephone number.</summary>
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
