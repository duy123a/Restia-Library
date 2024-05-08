using Restia.Common;
using Restia.Common.Abstractions.Infrastructure.DI;
using Restia.Common.Abstractions.Logger;
using Restia.Common.Configurations;
using Restia.Common.Infrastructure.DI;
using Restia.Common.Logger.SerilogLogger;
using Restia.Common.Utils;
using Restia.Playground.Commands;

namespace Restia.Playground;

internal class Program
{
	private static readonly IFileLogger _logger = new FileLogger();
	private static readonly IConfigurationService _configurationService = new ConfigurationService(new EnvironmentService());
	[STAThread]
	static void Main(string[] args)
	{
		try
		{
			var program = new Program();
			_logger.WriteInfo("Starting batch");

			// Excute program
			var isSuccess = ProcessUtility.ExecWithProcessMutex(program.Start);
			if (isSuccess == false)
			{
				throw new Exception("Startup failed because another process was running. Double activation is prohibited.");
			}

			_logger.WriteInfo("End batch normally");
		}
		catch (Exception ex)
		{
			_logger.WriteError(ex, string.Empty);
		}
	}

	private Program()
	{
		// Set some config here
		RestiaConfiguration.Logger = _configurationService.GetConfiguration<LoggerConfiguration>("FileLogging");
	}

	private void Start()
	{
		foreach (var command in CreateCommands())
		{
			command.Execute();
		}
	}

	private IEnumerable<ICommand> CreateCommands()
	{
		yield return new SampleCommand(_logger);
	}
}
