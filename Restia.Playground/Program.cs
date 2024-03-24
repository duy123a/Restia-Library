using Restia.Common.Logger;
using Restia.Common.Utils;
using Restia.Playground.Commands;

namespace Restia.Playground;

internal class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		try
		{
			var program = new Program();
			FileLogger.WriteInfo("Starting batch");

			// Excute program
			var isSuccess = ProcessUtility.ExecWithProcessMutex(program.Start);
			if (isSuccess == false)
			{
				throw new Exception("Startup failed because another process was running. Double activation is prohibited.");
			}

			FileLogger.WriteInfo("End batch normally");
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
