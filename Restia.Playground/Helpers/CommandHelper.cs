using Restia.Common.Utils;

namespace Restia.Playground.Helpers
{
    public static class CommandHelper
    {
        private const int ExitCommandValue = 0;
        private const int InvalidCommandValue = -1;

        public static void RunCommandLoop(
            List<(int CommandValue, string CommandTitle, Action Action)> commands,
            ref int successCount,
            ref int errorCount)
        {
            int commandValue = InvalidCommandValue;

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
                int tempCommandValue = InvalidCommandValue;
                string input;

                do
                {
                    Console.Write("Enter command: ");
                    input = Console.ReadLine() ?? string.Empty;
                    if (!int.TryParse(input, out tempCommandValue)) continue;

                    if (tempCommandValue == ExitCommandValue ||
                        commands.Any(command => command.CommandValue == tempCommandValue))
                    {
                        break;
                    }
                } while (true);

                if (tempCommandValue == ExitCommandValue)
                {
                    Console.WriteLine("Shut down");
                    Thread.Sleep(1000);
                    break;
                }

                // Invoke command
                commandValue = tempCommandValue;
                var executeCommand = commands.FirstOrDefault(x => x.CommandValue == commandValue);
                if (executeCommand == default)
                {
                    Console.WriteLine("Invalid command.");
                    continue;
                }

                Console.WriteLine("===================================");
                Console.WriteLine("Start");
                Console.WriteLine("===================================");
                Console.WriteLine();

                try
                {
                    executeCommand.Action.Invoke();
                    successCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }

                Console.WriteLine();
                Console.WriteLine("===================================");
                Console.WriteLine("End");
                Console.WriteLine("===================================");

                // Waiting for next action
                Console.Write("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        public static List<(int CommandValue, string CommandTitle, Action Action)> InitializeCommand(params Action[] actionList)
        {
            int index = 1;
            var commands = new List<(int CommandValue, string CommandTitle, Action Action)>();

            foreach (var action in actionList)
            {
                commands.Add((index++, StringUtility.AddSpacesBetweenUpperCharacter(action.Method.Name), action));
            }

            return commands;
        }
    }
}
