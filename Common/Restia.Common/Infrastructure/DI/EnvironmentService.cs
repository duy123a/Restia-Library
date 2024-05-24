using Restia.Common.Abstractions.Infrastructure.DI;
using System;

namespace Restia.Common.Infrastructure.DI
{
	public class EnvironmentService : IEnvironmentService
	{
		public EnvironmentService()
		{
			EnvironmentName = Environment.GetEnvironmentVariable("RESTIA_ENVIRONMENT")
				?? "development";
			EncryptionSingleSignOnKey = Environment.GetEnvironmentVariable("ENCRYPTION_SINGLE_SIGN_ON_KEY")
				?? "cmVzdGlhLWNoYW4=";
			EncryptionSingleSignOnIV = Environment.GetEnvironmentVariable("ENCRYPTION_SINGLE_SIGN_ON_IV")
				?? "dG9nZXRoZXIgZm9ydmVy";
		}

		public string EnvironmentName { get; set; }
		public string EncryptionSingleSignOnKey { get; set; }
		public string EncryptionSingleSignOnIV { get; set; }
	}
}
