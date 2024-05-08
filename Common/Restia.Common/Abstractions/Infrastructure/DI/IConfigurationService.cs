using Microsoft.Extensions.Options;

namespace Restia.Common.Abstractions.Infrastructure.DI
{
	public interface IConfigurationService
	{
		TConfiguration GetConfiguration<TConfiguration>(string sectionKey) where TConfiguration : new();

		IOptions<TConfiguration> GetOptions<TConfiguration>(string sectionKey) where TConfiguration : class, new();
	}
}
