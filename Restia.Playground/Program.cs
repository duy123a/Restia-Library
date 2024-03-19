using Restia.Common.Logger;
using Restia.Common.Utils;
using Restia.Playground.Commands;

namespace Restia.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		try
		{
			var program = new Program();
			FileLogger.WriteInfo("Starting");

			// Excute program
			var isSuccess = ProcessUtility.ExecWithProcessMutex(program.Start);
			if (isSuccess == false)
			{
				throw new Exception("Startup failed because another process was running. Double activation is prohibited.");
			}

			FileLogger.WriteInfo("End normally");
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
		}
	}

	private Program()
	{
		// Set some config here
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
		yield return new SampleCommand();
	}
}
