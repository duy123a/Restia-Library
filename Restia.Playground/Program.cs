using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restia.Common.Common;
using Restia.Common.Utils;
using Restia.Playground;
using Restia.Playground.Helpers;
using Restia.Playground.Methods;
using Serilog;

LoggerHelper.EnsureInitialized();
Log.Information("Console App booting up...");

try
{
    if (!ProcessUtility.ExecWithProcessMutex(async () =>
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<DbContext>(options =>
                {
                    options.UseInMemoryDatabase("Restia_Playground");
                });

                services.AddSingleton<ServiceLocator>();
                services.AddTransient<App>();
                services.AddServices();
            })
            .Build();

        var app = host.Services.GetRequiredService<App>();
        await app.RunAsync();
    }))
    {
        Console.WriteLine("Another instance is already running.");
        Environment.Exit(1);
    }
}
catch (Exception ex)
{
    LoggerHelper.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    LoggerHelper.EnsureInitialized();
    Log.Information("Console App shutting down...");
    Log.CloseAndFlush();
}
