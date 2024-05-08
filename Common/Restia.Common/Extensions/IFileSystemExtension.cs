using System.IO.Abstractions;

namespace Restia.Common.Extensions
{
	public static class IFileSystemExtension
	{
		public static void SafeDelete(this IFileSystem fileSystem, string path)
		{
			if (!fileSystem.File.Exists(path)) return;
			fileSystem.File.Delete(path);
		}
	}
}
