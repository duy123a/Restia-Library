using Restia.Common.Logger;
using Restia.Playground.Utils;

namespace Restia.Playground.Commands;
public class SampleCommand : CommandBase
{
	public SampleCommand(IFileLogger logger) : base(logger)
	{
		this.FileChecker = new FileChecker();
	}

	protected override void OnExecute()
	{
		// Get last execute date time
		var lastExecuteDateTime = this.FileChecker.GetLastExecuteDateTime();
		var executeDateTime = lastExecuteDateTime.HasValue
			? lastExecuteDateTime.Value
			: this.BeginDate;

		// Do somethings
		Console.WriteLine("Forever Together");
		_logger.WriteInfo("This is a {test} message", "test123a");

		// Update last execute date time file
		this.FileChecker.UpdateLastExecuteFile(this.BeginDate);
	}

	private FileChecker FileChecker { get; set; }
}
