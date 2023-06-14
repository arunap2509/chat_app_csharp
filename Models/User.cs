namespace ChatApp.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string DpUrl { get; set; }
    public List<UserActiveChat> ActiveChats { get; set; }
}
