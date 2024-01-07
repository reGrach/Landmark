using Landmark.ER301Driver.Abstract;

namespace Landmark.WebApp.Extensions
{
    public class DebugDriver : IDebugCallback
    {
        private readonly ILogger<MonitorReaderLoop> _logger;

        public DebugDriver(ILogger<MonitorReaderLoop> logger)
		{
            _logger = logger;
        }

        public void SendStatus(string status) => _logger.LogInformation(status);
    }
}

