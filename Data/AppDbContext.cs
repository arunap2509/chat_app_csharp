using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql().UseSnakeCaseNamingConvention();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserActiveChat> UserActiveChats { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupInfo> GroupInfos { get; set; }
    public DbSet<Chat> Chats { get; set; }
}
