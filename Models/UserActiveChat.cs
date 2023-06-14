namespace ChatApp.Models;

public class UserActiveChat
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public User User { get; set; }
    public string ChannelId { get; set; }
}
