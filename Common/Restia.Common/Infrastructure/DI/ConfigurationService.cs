using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Restia.Common.Abstractions.Infrastructure.DI;
using System.IO;
using System.Runtime.CompilerServices;

namespace Restia.Common.Infrastructure.DI
{
	public class ConfigurationService : IConfigurationService
	{
		private IConfigurationRoot? _cache;
		public ConfigurationService(IEnvironmentService envService)
		{
			this.EnvService = envService;
		}

		public TConfiguration GetConfiguration<TConfiguration>(string sectionKey)
			where TConfiguration : new()
		{
			var configuration = new TConfiguration();
			this.Cache.GetSection(sectionKey).Bind(configuration);

			return configuration;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IOptions<TConfiguration> GetOptions<TConfiguration>(string sectionKey)
			where TConfiguration : class, new()
		{
			return Options.Create(GetConfiguration<TConfiguration>(sectionKey));
		}

		public IConfigurationRoot Cache
		{
			get
			{
				if (_cache == null)
				{
					this.CurrentDirectory ??= Directory.GetCurrentDirectory();
					_cache = new ConfigurationBuilder()
						.SetBasePath(this.CurrentDirectory)
						.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
						.AddJsonFile($"appsettings.{EnvService.EnvironmentName}.json", optional: true)
						.AddEnvironmentVariables()
						.Build();
				}

				return _cache;
			}
		}

		public IEnvironmentService EnvService { get; }

		public string? CurrentDirectory { get; set; }
	}
}
