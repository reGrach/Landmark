using Landmark.Database;
using Landmark.ER301Driver;
using Landmark.ER301Driver.Abstract;
using Landmark.StartFinishApp.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Landmark.StartFinishApp;

class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        //builder.Services
        //    .AddDbContext<LandmarkContext>(options => options.UseNpgsql());

        builder.Services.AddSingleton<ITagDriver, TagDriver>();

        using IHost host = builder.Build();

        StartDevice(host.Services);

        await host.RunAsync();
    }

    static void StartDevice(IServiceProvider hostProvider)
    {
        using IServiceScope serviceScope = hostProvider.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        var driver = provider.GetRequiredService<ITagDriver>();

        driver.Start("/dev/cu.usbserial-0001");


        //ServiceLifetimeReporter logger = provider.GetRequiredService<ServiceLifetimeReporter>();
        //logger.ReportServiceLifetimeDetails(
        //    $"{lifetime}: Call 1 to provider.GetRequiredService<ServiceLifetimeReporter>()");

        //Console.WriteLine("...");

        //logger = provider.GetRequiredService<ServiceLifetimeReporter>();
        //logger.ReportServiceLifetimeDetails(
        //    $"{lifetime}: Call 2 to provider.GetRequiredService<ServiceLifetimeReporter>()");

        Console.WriteLine();
    }

    static void ConfigureServices(IServiceCollection services)
    {
        // configure logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        // build config
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        services.Configure<AppSettings>(configuration.GetSection("App"));

        // add services:
        // services.AddTransient<IMyRespository, MyConcreteRepository>();

        // add app
        services.AddTransient<TagDriver>();
    }
}

