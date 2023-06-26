using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models;

public class Friend
{
    public int Id { get; set; }
    public string Userd { get; set; }
    public User User { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string> FriendsIds { get; set; }
}
