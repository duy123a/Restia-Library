using System.IO.Abstractions;

namespace Restia.Playground.Utils;
public class FileChecker
{
	private readonly IFileSystem _fileSystem;
	public FileChecker() : this(fileSystem: new FileSystem())
	{
	}

	public FileChecker(IFileSystem fileSystem)
	{
		_fileSystem = fileSystem;

		this.LastExecuteFilePath = _fileSystem.Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			Constants.TMP_DIRECTORY_NAME,
			Constants.FILENAME_LASTEXEC);
	}

	public void UpdateLastExecuteFile(DateTimeOffset date)
	{
		// Delete old file
		if (_fileSystem.File.Exists(this.LastExecuteFilePath))
		{
			_fileSystem.File.Delete(this.LastExecuteFilePath);
		}

		// Create new file
		var directoryPath = _fileSystem.Path.GetDirectoryName(this.LastExecuteFilePath);
		if (directoryPath != null && _fileSystem.Directory.Exists(directoryPath) == false)
		{
			_fileSystem.Directory.CreateDirectory(directoryPath);
		}
		_fileSystem.File.WriteAllText(
			this.LastExecuteFilePath,
			date.ToString(Constants.FILECONTENT_LASTEXEC_DATEFORMAT));
	}

	public DateTimeOffset? GetLastExecuteDateTime()
	{
		try
		{
			if (_fileSystem.File.Exists(this.LastExecuteFilePath) == false) return null;

			var lastExecuteDateTimeString = _fileSystem.File.ReadAllText(this.LastExecuteFilePath);
			var lastExecuteDateTime = DateTimeOffset.ParseExact(
				lastExecuteDateTimeString.Trim(),
				Constants.FILECONTENT_LASTEXEC_DATEFORMAT,
				null);
			return lastExecuteDateTime;
		}
		catch
		{
			return null;
		}
	}

	private string LastExecuteFilePath { get; set; }
}
