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
		this.LastExecuteFilePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			Constants.TMP_DIRECTORY_NAME,
			Constants.FILENAME_LASTEXEC);

		_fileSystem = fileSystem;
	}

	public void UpdateLastExecuteFile(DateTime date)
	{
		// Delete old file
		if (_fileSystem.File.Exists(this.LastExecuteFilePath))
		{
			_fileSystem.File.Delete(this.LastExecuteFilePath);
		}

		// Create new file
		var directoryPath = Path.GetDirectoryName(this.LastExecuteFilePath);
		if (directoryPath != null && Directory.Exists(directoryPath) == false)
		{
			Directory.CreateDirectory(directoryPath);
		}
		_fileSystem.File.WriteAllText(
			this.LastExecuteFilePath,
			date.ToString(Constants.FILECONTENT_LASTEXEC_DATEFORMAT));
	}

	public DateTime? GetLastExecuteDateTime()
	{
		try
		{
			if (_fileSystem.File.Exists(this.LastExecuteFilePath) == false) return null;

			var lastExecuteDateTimeString = File.ReadAllText(this.LastExecuteFilePath);
			var lastExecuteDateTime = DateTime.ParseExact(
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
