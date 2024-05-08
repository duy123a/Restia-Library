using Restia.Common.Extensions;
using Restia.Common.Logger;
using Restia.Playground.Helpers;
using Restia.Playground.Utils;

namespace Restia.Playground.Commands;
public class SampleCommand : CommandBase
{
	private DateTimeOffset _lastExecuteDateTime;
	public SampleCommand(IFileLogger logger) : base(logger)
	{
		this.FileChecker = new FileChecker();
	}

	protected override void OnExecute()
	{
		// Get last execute date time
		var lastExecuteDateTime = this.FileChecker.GetLastExecuteDateTime();
		_lastExecuteDateTime = lastExecuteDateTime ?? this.BeginDate;

		// Do somethings
		var commands = CommandHelper.InitializeCommand(HelloWorld);
		int successCount = 0;
		int errorCount = 0;
		CommandHelper.InitializeCommandList(commands, ref successCount, ref errorCount);

		this.SuccessCount += successCount;
		this.ErrorCount += errorCount;

		// Update last execute date time file
		this.FileChecker.UpdateLastExecuteFile(this.BeginDate);
	}

	private void HelloWorld()
	{
		Console.WriteLine("The end of the world at {0}, the new dawn at {1}", _lastExecuteDateTime.ToDateString(), this.BeginDate.ToDateString());
		_logger.WriteInfo("The end of the world at {0}, the new dawn at {1}", _lastExecuteDateTime.ToDateString(), this.BeginDate.ToDateString());
	}

	private FileChecker FileChecker { get; set; }
}
