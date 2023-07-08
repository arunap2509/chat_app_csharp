using ChatApp.Enum;

namespace ChatApp.Models;

public class Thread
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public User User { get; set; }
    public string ChannelId { get; set; }
    public ChannelType Type { get; set; }
    public ChatApp.Enum.ThreadState State { get; set; }
}
