namespace ChatApp.Dto;

public class CreateGroupRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string DpUrl { get; set; }
    public string CreatedByUser { get; set; }
    public List<string> UserIds { get; set; }
}
