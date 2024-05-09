using Microsoft.Extensions.Options;
using System;

namespace Restia.Common.Abstractions.Infrastructure.DI
{
	public interface IConfigurationService
	{
		void InitializeLibraryConfig();

		TConfiguration GetConfiguration<TConfiguration>(string sectionKey) where TConfiguration : new();

		object? GetConfiguration(Type configurationType, string sectionKey);

		IOptions<TConfiguration> GetOptions<TConfiguration>(string sectionKey) where TConfiguration : class, new();
	}
}
