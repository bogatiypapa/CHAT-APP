using Microsoft.EntityFrameworkCore;

public class ChatDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSubscription> Subscriptions { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=chat.db");
}
