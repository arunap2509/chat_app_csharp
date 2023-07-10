using ChatApp.Enum;

namespace ChatApp.Dto;

public class ThreadPreviewResponse
{
    public string ChannelId { get; set; }
    public ChannelType ChannelType { get; set; }
    public string Name { get; set; }
    public string DpUrl { get; set; }
    public string Description { get; set; }
    public ChatApp.Enum.ThreadState ThreadState { get; set; }
}
