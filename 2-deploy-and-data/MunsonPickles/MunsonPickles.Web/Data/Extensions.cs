using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

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
        var client = new SecretClient(new Uri($"https://{configuration["KeyVaultName"]}.vault.azure.net/"), new DefaultAzureCredential());

        var sqlConnection = client.GetSecret("WebReviewSqlDb").Value.Value;

        //var sqlConnection = configuration["ConnectionStrings:WebReview:SqlDb"];

        services.AddSqlServer<PickleDbContext>(sqlConnection, options => options.EnableRetryOnFailure());

    }
}
