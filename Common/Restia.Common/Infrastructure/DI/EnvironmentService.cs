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
		}

		public string EnvironmentName { get; set; }
	}
}
