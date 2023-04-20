using BiliConnect;

using Birai.Data;
using Birai.Services;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"));
    });

    builder.Services.AddDataProtection().UseCryptographicAlgorithms(
        new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256,
        });

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddBilibiliConnect(options =>
        {
            options.CookieSavePath = "./cookies.txt";
            options.UseProxy = true;
            options.ProxyUrl = "http://localhost:7890";
        });
    }
    else
    {
        builder.Services.AddBilibiliConnect(options =>
        {
            options.CookieSavePath = "./cookies.txt";
        });
    }


    // register analyzers
    {
        var analyzers = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => typeof(IMessageAnalyzer).IsAssignableFrom(p) && p.IsClass);

        foreach (var analyzer in analyzers)
        {
            builder.Services.AddSingleton(typeof(IMessageAnalyzer), analyzer);
        }
    }

    builder.Services.AddHostedService<BotService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //app.UseHsts();
    }

    using var scope = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope();
    var db = scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db?.Database.EnsureCreated();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
