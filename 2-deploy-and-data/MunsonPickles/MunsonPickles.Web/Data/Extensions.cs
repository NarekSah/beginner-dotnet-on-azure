using Microsoft.EntityFrameworkCore;

namespace MunsonPickles.Web.Data;

public static class Extensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var pickleContext = services.GetRequiredService<PickleDbContext>();
        
        pickleContext.Database.EnsureCreated();        

        DBInitializer.InitializeDatabase(pickleContext);        
    }

    public static void AddDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the connection string
        var connectionString = configuration.GetConnectionString("WebReviewSqlDb");

        // Register DbContext
        services.AddDbContext<PickleDbContext>(options =>
            options.UseSqlServer(connectionString));

    }
}
