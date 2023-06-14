using ChatApp.Enum;

namespace ChatApp.Models;

public class Chat
{
    public int Id { get; set; } // check if you can come up with a way to make this sequential, increasing
    public string UserId { get; set; }
    public string ChannelId { get; set; }
    public string Message { get; set; }
    public MessageType MessageType { get; set; }
    public bool Seen { get; set; }
}
