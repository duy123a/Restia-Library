using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restia.Playground.Helpers;
using Restia.Playground.Methods;

namespace Restia.Playground
{
    public class App
    {
        private readonly ILogger _logger;
        private readonly DbContext _db;
        private readonly ServiceLocator _serviceLocator;
        private int _successCount = 0;
        private int _errorCount = 0;

        public App(
            ILogger<App> logger,
            DbContext db,
            ServiceLocator serviceLocator)
        {
            _logger = logger;
            _db = db;
            _serviceLocator = serviceLocator;
        }

        public async Task RunAsync()
        {
            CommandHelper.RunCommandLoop(_serviceLocator, ref _successCount, ref _errorCount);

            await Task.CompletedTask;
        }
    }
}
