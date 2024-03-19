using Restia.Common.Logger;
using System.Text;

namespace Restia.Playground.Commands;
public abstract class CommandBase : ICommand
{
	public CommandBase()
	{
		this.ActionName = this.GetType().Name.Replace("Command", string.Empty);
	}

	public void Execute()
	{
		OnStart();
		try
		{
			OnExecute();
			OnComplete();
		}
		catch (Exception ex)
		{
			this.ErrorMessages.AppendLine(BaseLogger.CreateExceptionMessage(ex));
			FileLogger.WriteError(ex);

			OnError();
		}

		OnEnd();
	}

	protected virtual void OnStart()
	{
		this.BeginDate = DateTime.Now;
		this.SuccessCount = 0;
		this.ErrorCount = 0;
		this.ErrorMessages = new StringBuilder();
	}

	protected abstract void OnExecute();

	protected virtual void OnError()
	{
		this.EndDate = DateTime.Now;
	}

	protected virtual void OnComplete()
	{
		this.EndDate = DateTime.Now;
	}

	protected virtual void OnEnd()
	{
		WriteInfoLog();
	}

	private void WriteInfoLog()
	{
		var message = string.Format(
			"{0} Time：{1}（Successful：{2}times、Failed：{3}times）",
			(this.ActionName + " completion").PadRight(25, '　'),
			this.EndDate - this.BeginDate,
			this.SuccessCount.ToString().PadLeft(3, ' '),
			this.ErrorCount.ToString().PadLeft(3, ' '));
		FileLogger.WriteInfo(message);
	}

	protected DateTime BeginDate { get; set; }
	protected DateTime EndDate { get; set; }
	protected string ActionName { get; set; } = string.Empty;
	protected int SuccessCount { get; set; }
	protected int ErrorCount { get; set; }
	protected StringBuilder ErrorMessages { get; set; } = new StringBuilder();
	protected bool HasError => string.IsNullOrEmpty(this.ErrorMessages.ToString().Trim()) == false;
}
