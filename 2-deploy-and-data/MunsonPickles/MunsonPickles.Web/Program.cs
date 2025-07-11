using Azure.Identity;

using MunsonPickles.Events;
using MunsonPickles.Web.Data;
using MunsonPickles.Web.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IEventGridPublisher, EventGridPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.CreateDbIfNotExists();

app.Run();
