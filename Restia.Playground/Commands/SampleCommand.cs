using Restia.Common.Extensions;
using Restia.Common.Logger;
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
		_lastExecuteDateTime = lastExecuteDateTime.HasValue
			? lastExecuteDateTime.Value
			: this.BeginDate;

		// Do somethings
		InitializeSampleCommand();

		// Update last execute date time file
		this.FileChecker.UpdateLastExecuteFile(this.BeginDate);
	}

	private void InitializeSampleCommand()
	{
		var commandValue = -1;
		var commands = InitializeCommand();
		while (true)
		{
			Console.WriteLine("===================================");
			Console.WriteLine("Command list");
			Console.WriteLine("===================================");
			foreach (var command in commands)
			{
				Console.WriteLine($"[{command.CommandValue}] {command.CommandTitle}");
			}
			Console.WriteLine("[0] End");

			// Get command
			var input = string.Empty;
			var tempCommandValue = -1;

			do
			{
				Console.Write("Enter command: ");
				input = Console.ReadLine();
				if ((int.TryParse(input, out tempCommandValue) == false)) continue;

				if (tempCommandValue == 0
					|| commands.Any(command => command.CommandValue == tempCommandValue))
				{
					break;
				}
			} while (true);
			if (tempCommandValue == 0)
			{
				Console.WriteLine("Shut down");
				Thread.Sleep(1000);
				break;
			}

			// Invoke command
			commandValue = tempCommandValue;
			var executeCommand = commands.First(x => x.CommandValue == commandValue);
			Console.WriteLine("===================================");
			Console.WriteLine("Start");
			Console.WriteLine("===================================");
			Console.WriteLine();
			try
			{
				executeCommand.Action.Invoke();
				this.SuccessCount++;
			}
			catch (Exception)
			{
				this.ErrorCount++;
				throw;
			}
			Console.WriteLine();
			Console.WriteLine("===================================");
			Console.WriteLine("End");
			Console.WriteLine("===================================");

			// Waiting for next action
			Console.Write("Press to continute...");
			Console.ReadKey();
			Console.Clear();
		}
	}

	private List<(int CommandValue, string CommandTitle, Action Action)> InitializeCommand()
	{
		var index = 1;
		var commands = new List<(int CommandValue, string CommandTitle, Action Action)>
		{
			(index++, "Hello World", HelloWorld),
		};

		return commands;
	}

	private void HelloWorld()
	{
		Console.WriteLine("The end of the world at {0}, the new dawn at {1}", _lastExecuteDateTime.ToDateString(), this.BeginDate.ToDateString());
		_logger.WriteInfo("The end of the world at {0}, the new dawn at {1}", _lastExecuteDateTime.ToDateString(), this.BeginDate.ToDateString());
	}

	private FileChecker FileChecker { get; set; }
}
