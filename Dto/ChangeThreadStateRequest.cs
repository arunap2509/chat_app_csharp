namespace ChatApp.Dto;

public class ChangeThreadStateRequest
{
    public string ChannelId { get; set; }
    public short State { get; set; }
    public short ChannelType { get; set; }
}
