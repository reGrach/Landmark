using System.Collections.Concurrent;
using Landmark.Database;
using Landmark.Database.Enums;
using Landmark.Database.Model;
using Landmark.ER301Driver;

namespace Landmark.WebApp.Extensions
{
    public class MonitorReaderLoop
    {
        private ConcurrentDictionary<string, DateTime> accumulationSerial = new();

        private readonly TagDriver tagDriver;
        private readonly ILogger<MonitorReaderLoop> _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly IServiceScopeFactory _scopeFactory;

        public MonitorReaderLoop(ILogger<MonitorReaderLoop> logger, IServiceScopeFactory scopeFactory, IHostApplicationLifetime applicationLifetime)
        {
            var debugger = new DebugDriver(logger);
            _logger = logger;
            tagDriver = new TagDriver(debugger);
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _scopeFactory = scopeFactory;
        }

        public void Start()
        {
            tagDriver.Start("/dev/cu.usbserial-0001");
            Task.Run(CleanUpSerial);
            Task.Run(async () => await MonitorAsync());
        }

        private async ValueTask MonitorAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var serial = tagDriver.Detection();

                if (!string.IsNullOrEmpty(serial) && accumulationSerial.TryAdd(serial, DateTime.Now))
                    await HandleSerial(serial);
            }
        }

        private void CleanUpSerial()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                accumulationSerial = new(accumulationSerial.Where(x => DateTime.Now.Subtract(x.Value).Minutes > 1));
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private async ValueTask HandleSerial(string serial)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<LandmarkContext>();
                    var dbRace = context.Races.FirstOrDefault(x => x.SerialTag == serial);

                    if (dbRace is null)
                    {
                        var countTeam = context.Races.Count();

                        await context.Races.AddAsync(new Race
                        {
                            TeamNumber = countTeam + 1,
                            CountPoints = 0,
                            SexType = SexType.Male,
                            SerialTag = serial,
                        });
                    }
                    else
                    {
                        if (dbRace.FinishTime.HasValue)
                        {
                            _logger.LogInformation($"Race (id: {dbRace.Id}) has already finished");
                        }
                        else
                        {
                            if (dbRace.StartTime.HasValue)
                            {
                                dbRace.FinishTime = TimeOnly.FromDateTime(DateTime.Now);
                            }
                            else
                            {
                                dbRace.StartTime = TimeOnly.FromDateTime(DateTime.Now);
                            }
                        }
                    }

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<MonitorReaderLoop>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}

