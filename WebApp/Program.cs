using Landmark.Database;
using Landmark.WebApp.Extensions;
using Landmark.WebApp.Hubs;
using Microsoft.EntityFrameworkCore;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<MonitorReaderLoop>();

        builder.Services.AddDbContext<LandmarkContext>(options =>
            options
            .UseNpgsql(builder.Configuration.GetConnectionString("LandmarkContext"))
            .UseSnakeCaseNamingConvention());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapHub<MainHub>("/chatHub");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        var monitorLoop = app.Services.GetRequiredService<MonitorReaderLoop>();
        monitorLoop.Start();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<LandmarkContext>();
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        app.Run();
    }
}

