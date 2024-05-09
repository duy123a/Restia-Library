using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Restia.Common.Abstractions.Infrastructure.DI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PropertyCache = Restia.Common.Caching.Cache<System.Type, System.Collections.Generic.Dictionary<string, System.Reflection.PropertyInfo>>;

namespace Restia.Common.Infrastructure.DI
{
	public class ConfigurationService : IConfigurationService
	{
		private IConfigurationRoot? _cache;
		private static readonly PropertyCache _propertyCache = new PropertyCache();
		public ConfigurationService(IEnvironmentService envService)
		{
			this.EnvService = envService;
		}

		public void InitializeLibraryConfig()
		{
			var properties = GetProperties(typeof(RestiaConfiguration));
			if (properties == null) return;
			foreach (var property in properties)
			{
				var propertyType = property.Value.PropertyType;
				property.Value.SetValue(null, GetConfiguration(propertyType, property.Key));
			}
		}

		public TConfiguration GetConfiguration<TConfiguration>(string sectionKey)
			where TConfiguration : new()
		{
			// Microsoft has an extensive method get (IConfigurationRoot) which works similar to it
			var configuration = new TConfiguration();
			this.Cache.GetSection(sectionKey).Bind(configuration);

			return configuration;
		}

		public object? GetConfiguration(Type configurationType, string sectionKey)
		{
			// Since C# doesn't support variable for generic type param, we can work around like this
			var configuration = Activator.CreateInstance(configurationType);
			this.Cache.GetSection(sectionKey).Bind(configuration);

			return configuration;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IOptions<TConfiguration> GetOptions<TConfiguration>(string sectionKey)
			where TConfiguration : class, new()
		{
			return Options.Create(GetConfiguration<TConfiguration>(sectionKey));
		}

		public static Dictionary<string, PropertyInfo> GetProperties(Type type)
		{
			// Caching PropertyInfo for quick lookup instead of using reflection each time.
			if (!_propertyCache.TryGetValue(type, out var properties))
			{
				properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static).ToDictionary(p => p.Name, p => p);
				_propertyCache.TryAdd(type, () => properties);
			}

			if (properties == null)
			{
				throw new Exception(
					$"The object of type {type.Name} doesn't have any public properties");
			}

			return properties;
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
