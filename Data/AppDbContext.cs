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

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //     modelBuilder.Entity<User>()
    //     .Property(u => u.CreatedAt)
    //     .HasDefaultValueSql("now()");


    // }

    public DbSet<User> Users { get; set; }
    public DbSet<Models.Thread> Threads { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMemberInfo> GroupMemberInfos { get; set; }
    public DbSet<Chat> Chats { get; set; }
    // public DbSet<Friend> Friends { get; set; }
    public DbSet<OtpStore> OtpStores { get; set; }
}
