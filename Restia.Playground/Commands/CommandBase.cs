using Restia.Common.Abstractions.Logger;
using Restia.Common.Infrastructure.Security;
using System.Text;

namespace Restia.Playground.Commands;
public abstract class CommandBase : ICommand
{
	protected readonly IFileLogger _logger;
	protected readonly ICryptographyService _cryptographyService;
	public CommandBase(IFileLogger logger, ICryptographyService cryptographyService)
	{
		this.ActionName = this.GetType().Name.Replace("Command", " Batch");
		_logger = logger;
		_cryptographyService = cryptographyService;
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
			_logger.WriteError(ex, string.Empty);

			OnError();
		}

		OnEnd();
	}

	protected virtual void OnStart()
	{
		this.BeginDate = DateTimeOffset.Now;
		this.SuccessCount = 0;
		this.ErrorCount = 0;
		this.ErrorMessages = new StringBuilder();
	}

	protected abstract void OnExecute();

	protected virtual void OnError()
	{
		this.EndDate = DateTimeOffset.Now;
	}

	protected virtual void OnComplete()
	{
		this.EndDate = DateTimeOffset.Now;
	}

	protected virtual void OnEnd()
	{
		WriteInfoLog();
	}

	private void WriteInfoLog()
	{
		var message = string.Format(
			"{0} Time: {1} (Successful: {2} times, Failed: {3} times)",
			(this.ActionName + " completion.").PadRight(15, ' '),
			this.EndDate - this.BeginDate,
			this.SuccessCount.ToString().PadLeft(1, ' '),
			this.ErrorCount.ToString().PadLeft(1, ' '));
		_logger.WriteInfo(message);
	}

	protected DateTimeOffset BeginDate { get; set; }
	protected DateTimeOffset EndDate { get; set; }
	protected string ActionName { get; set; } = string.Empty;
	protected int SuccessCount { get; set; }
	protected int ErrorCount { get; set; }
	protected StringBuilder ErrorMessages { get; set; } = new StringBuilder();
	protected bool HasError => string.IsNullOrEmpty(this.ErrorMessages.ToString().Trim()) == false;
}
