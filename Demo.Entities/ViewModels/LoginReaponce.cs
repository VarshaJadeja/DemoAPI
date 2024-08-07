using Demo.Entities.Entities;

namespace Demo.Entities.ViewModels;

public class LoginResponse
{
    public User User { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
