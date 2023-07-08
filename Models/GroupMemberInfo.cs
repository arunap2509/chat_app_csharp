namespace ChatApp.Models;

public class GroupMemberInfo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GroupId { get; set; }
    public Group Group { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public bool IsAdmin { get; set; }
}
