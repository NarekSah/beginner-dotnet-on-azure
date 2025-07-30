using Microsoft.EntityFrameworkCore;

namespace MunsonPickles.Web.Data;

public static class DBExtensions
{
    public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnection = configuration.GetConnectionString("WebReviewSqlDb");
        services.AddDbContext<PickleDbContext>(options => options.UseSqlServer(sqlConnection));
        return services;
    }

    public static WebApplication CreateDbIfNotExists(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<PickleDbContext>();
                context.Database.EnsureCreated();
                DBInitializer.InitializeDatabase(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }
        return app;
    }
}