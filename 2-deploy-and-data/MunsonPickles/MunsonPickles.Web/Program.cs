using Azure.Identity;
using Microsoft.AspNetCore.HttpLogging;
using System.Text;

using MunsonPickles.Events;
using MunsonPickles.Web.Data;
using MunsonPickles.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add HTTP logging to log all requests
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();



//if (!builder.Environment.IsDevelopment())
//{
//    // Load secrets from Azure Key Vault in production
//    var keyVaultEndpoint = new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/");
//    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
//}

builder.Services.AddDBContext(builder.Configuration);

builder.Services.AddTransient<ProductService>();
builder.Services.AddTransient<ReviewService>();
builder.Services.AddScoped<LoggingService>();

builder.Services.AddScoped<IEventGridPublisher, EventGridPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.Logger.LogInformation("Development environment detected");
}

// Use HTTP logging middleware
app.UseHttpLogging();

// Add a simple request logging middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

    // Capture the start time
    var startTime = DateTime.UtcNow;

    // Call the next middleware
    await next();

    // Log response information
    var elapsedTime = DateTime.UtcNow - startTime;
    logger.LogInformation("Response: {StatusCode} completed in {ElapsedMilliseconds}ms",
        context.Response.StatusCode, elapsedTime.TotalMilliseconds);
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Logger.LogInformation("Application starting, initializing database if needed");
app.CreateDbIfNotExists();
app.Logger.LogInformation("Database initialization completed");

app.Run();
