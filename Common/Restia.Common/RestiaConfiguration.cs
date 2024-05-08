using Restia.Common.Configurations;

namespace Restia.Common
{
	public sealed class RestiaConfiguration
	{
		public static LoggerConfiguration Logger { get; set; } = new LoggerConfiguration();
	}
}
