using Restia.Common.Logger;
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
			FileLogger.WriteInfo("起動");

			// Excute program
			var isSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
			if (isSuccess == false)
			{
				throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
			}

			FileLogger.WriteInfo("正常終了");
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
