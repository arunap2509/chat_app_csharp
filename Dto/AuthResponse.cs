namespace ChatApp.Dto;

public class AuthResponse
{
    public string UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
