using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data;

public static class DbContextSetUp
{
    public static void RunMigration(this IServiceCollection services, IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
