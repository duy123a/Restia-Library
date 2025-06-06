using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restia.Playground.Methods;

namespace Restia.Playground
{
    public class App
    {
        private readonly ILogger _logger;
        private readonly DbContext _db;
        private readonly ServiceLocator _serviceLocator;

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
            _serviceLocator.GenerateRsaKeyPair();
            await Task.CompletedTask;
        }
    }
}
