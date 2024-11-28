using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class DesignTimeDbFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=./app.db");
            })
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<AppDbContext>();
    }
}