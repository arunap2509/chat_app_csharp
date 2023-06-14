namespace ChatApp.Models;

public class Group
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string DpUrl { get; set; }
    public List<GroupInfo> GroupInfos { get; set; }
}
