namespace Restia.Playground.Utils;
public class FileChecker
{
	public FileChecker()
	{
		this.LastExecuteFilePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			Constants.TMP_DIRECTORY_NAME,
			Constants.FILENAME_LASTEXEC);
	}

	public void UpdateLastExecuteFile(DateTime date)
	{
		// Delete old file
		if (File.Exists(this.LastExecuteFilePath))
		{
			File.Delete(this.LastExecuteFilePath);
		}

		// Create new file
		var directoryPath = Path.GetDirectoryName(this.LastExecuteFilePath);
		if (directoryPath != null && Directory.Exists(directoryPath) == false)
		{
			Directory.CreateDirectory(directoryPath);
		}
		File.WriteAllText(
			this.LastExecuteFilePath,
			date.ToString(Constants.FILECONTENT_LASTEXEC_DATEFORMAT));
	}

	public DateTime? GetLastExecuteDateTime()
	{
		try
		{
			if (File.Exists(this.LastExecuteFilePath) == false) return null;

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
