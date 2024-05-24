namespace Restia.Common.Abstractions.Infrastructure.DI
{
	public interface IEnvironmentService
	{
		string EnvironmentName { get; set; }
		string EncryptionSingleSignOnKey { get; set; }
		string EncryptionSingleSignOnIV { get; set; }
	}
}
