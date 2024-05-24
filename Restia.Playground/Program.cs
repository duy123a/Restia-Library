using Restia.Common.Abstractions.Infrastructure.DI;
using Restia.Common.Abstractions.Logger;
using Restia.Common.Infrastructure.DI;
using Restia.Common.Infrastructure.Security;
using Restia.Common.Logger.SerilogLogger;
using Restia.Common.Utils;
using Restia.Playground.Commands;

namespace Restia.Playground;

internal class Program
{
	private static readonly IEnvironmentService _environmentService = new EnvironmentService();
	private static readonly IFileLogger _logger = new FileLogger();
	private static readonly IConfigurationService _configurationService = new ConfigurationService(_environmentService);
	private static readonly ICryptographyService _cryptographyService = new CryptographyService(_environmentService);
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
		_configurationService.InitializeLibraryConfig();
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
		yield return new SampleCommand(_logger, _cryptographyService);
	}
}
