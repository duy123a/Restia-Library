using Restia.Common.Utils;

namespace Restia.Playground.Helpers;
public class CommandHelper
{
	public static void InitializeCommandList(
		List<(int CommandValue, string CommandTitle, Action Action)> commands,
		ref int successCount,
		ref int errorCount)
	{
		var commandValue = -1;
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
				successCount++;
			}
			catch (Exception)
			{
				errorCount++;
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

	public static List<(int CommandValue, string CommandTitle, Action Action)> InitializeCommand(params Action[] actionList)
	{
		var index = 1;
		var commands = new List<(int CommandValue, string CommandTitle, Action Action)>();

		foreach (var action in actionList)
		{
			commands.Add((index++, StringUtility.AddSpacesBetweenUpperCharacter(action.Method.Name), action));
		}

		return commands;
	}
}
