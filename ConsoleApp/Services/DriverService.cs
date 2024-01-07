using System;
using Landmark.ER301Driver;
using Landmark.StartFinishApp.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Landmark.StartFinishApp.Services
{
	public class DriverService : RootCommand
	{
		

        public DriverService(IOptions<AppSettings> appSettings, ILogger<TagDriver> logger)
		{

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

            var nameArgument = new Argument<string>("name", "The name of the person to greet.");
            AddArgument(nameArgument);

            this.SetHandler(Execute, nameArgument);



        }
    }
}

