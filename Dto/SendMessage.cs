namespace ChatApp.Dto;

public class SendMessage
{
    public string Message { get; set; }
    public string ChannelId { get; set; }
    public short ChannelType { get; set; }
}
