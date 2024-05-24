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
				?? "AAECAwQFBgcICQoLDA0ODw==";
			EncryptionSingleSignOnIV = Environment.GetEnvironmentVariable("ENCRYPTION_SINGLE_SIGN_ON_IV")
				?? "UjJvchPnPof5w2P9e9ThzA==";
		}

		public string EnvironmentName { get; set; }
		public string EncryptionSingleSignOnKey { get; set; }
		public string EncryptionSingleSignOnIV { get; set; }
	}
}
