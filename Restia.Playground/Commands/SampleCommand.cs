using Restia.Common;
using Restia.Common.Extensions;
using Restia.Common.Logger;
using Restia.Common.Utils;
using Restia.Playground.Attribute;
using Restia.Playground.Class;
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
		var actionList = new Action[]
		{
			HelloWorld,
			FindLoggingFilePath,
			FindNameOfSampleAttribute
		};

		var commands = CommandHelper.InitializeCommand(actionList);
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

	private void FindLoggingFilePath()
	{
		var loggingFilePath = StringUtility.ToEmpty(GlobalConfiguration.Logger.GetPropertyValue("LOG_DIR_FILE_PATH"));
		Console.WriteLine("This is current logging file path: {0}", loggingFilePath);
		_logger.WriteInfo("This is current logging file path: {0}", loggingFilePath);
	}

	private void FindNameOfSampleAttribute()
	{
		var sample = new SampleClass();
		var sampleAttribute = sample.GetCustomAttribute<SampleAttribute>();
		if (sampleAttribute == null)
		{
			Console.WriteLine("Can't find name of sample attribute");
		}
		else
		{
			Console.WriteLine("This is name of sample attribute: {0}", sampleAttribute.Name);
			_logger.WriteInfo("This is name of sample attribute: {0}", sampleAttribute.Name);
		}
	}

	private FileChecker FileChecker { get; set; }
}
