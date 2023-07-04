namespace ChatApp.Models;

public class OtpStore
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Otp { get; set; }
    public DateTime ExpiresAt { get; set; }
}
