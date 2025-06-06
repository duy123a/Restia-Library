using Microsoft.Extensions.Configuration;
using Serilog;

namespace Restia.Playground.Helpers
{
    public static class LoggerHelper
    {
        public static void EnsureInitialized()
        {
            if (Log.Logger is not Serilog.Core.Logger)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();
            }
        }
    }
}
