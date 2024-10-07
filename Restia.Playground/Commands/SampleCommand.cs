using Restia.Common;
using Restia.Common.Abstractions.Logger;
using Restia.Common.Extensions;
using Restia.Common.Infrastructure.Security;
using Restia.Common.Utils;
using Restia.Playground.Attribute;
using Restia.Playground.Class;
using Restia.Playground.Helpers;
using Restia.Playground.Utils;

namespace Restia.Playground.Commands;
public class SampleCommand : CommandBase
{
	private DateTimeOffset _lastExecuteDateTime;
	public SampleCommand(IFileLogger logger, ICryptographyService cryptographyService) : base(logger, cryptographyService)
	{
		this.FileChecker = new FileChecker();
	}

	protected override void OnExecute()
	{
		// Get last execute date time
		var lastExecuteDateTime = this.FileChecker.GetLastExecuteDateTime();
		_lastExecuteDateTime = lastExecuteDateTime ?? this.BeginDate;

		// Do somethings
		var actionList = new Action[]
		{
			HelloWorld,
			FindLoggingFilePath,
			FindNameOfSampleAttribute,
			GenerateRandomCryptoValue,
			EncryptPassword,
			DecryptPassword,
			CreateBase64String,
			DecryptBase64String,
			ResolvePath
		};

		var commands = CommandHelper.InitializeCommand(actionList);
		int successCount = 0;
		int errorCount = 0;

		CommandHelper.InitializeCommandList(commands, ref successCount, ref errorCount);

		this.SuccessCount += successCount;
		this.ErrorCount += errorCount;

		// Update last execute date time file
		this.FileChecker.UpdateLastExecuteFile(this.BeginDate);
	}

	private void HelloWorld()
	{
		WriteResultLog("The end of the world at {0}, the new dawn at {1}", _lastExecuteDateTime.ToDateString(), this.BeginDate.ToDateString());
	}

	private void FindLoggingFilePath()
	{
		var loggingFilePath = StringUtility.ToEmpty(RestiaConfiguration.Logger.GetPropertyValue("LOG_DIR_FILE_PATH"));
		WriteResultLog("This is current logging file path: {0}", loggingFilePath);
	}

	private void FindNameOfSampleAttribute()
	{
		var sample = new SampleClass();
		var sampleAttribute = sample.GetCustomAttribute<SampleAttribute>();
		if (sampleAttribute == null)
		{
			Console.WriteLine("Can't find name of sample attribute");
		}
		else
		{
			WriteResultLog("This is name of sample attribute: {0}", sampleAttribute.Name);
		}
	}

	private void EncryptPassword()
	{
		string? password = string.Empty;

		// Loop until a non-empty password is provided
		while (string.IsNullOrEmpty(password))
		{
			Console.WriteLine("Enter your password");
			password = Console.ReadLine();

			if (string.IsNullOrEmpty(password))
			{
				Console.WriteLine("Password can't be empty. Please try again.");
			}
		}

		var encryptedPassword = _cryptographyService.Encrypt(password);

		WriteResultLog("This is your encrypted password: {0}", encryptedPassword);
	}

	private void GenerateRandomCryptoValue()
	{
		var randomCryptoValue = _cryptographyService.GenerateRandomCryptoValue();
		WriteResultLog("This is your random crypto value: {0}", randomCryptoValue);
	}

	private void DecryptPassword()
	{
		string? password = string.Empty;

		// Loop until a non-empty password is provided
		while (string.IsNullOrEmpty(password))
		{
			Console.WriteLine("Enter your encrypted password");
			password = Console.ReadLine();

			if (string.IsNullOrEmpty(password))
			{
				Console.WriteLine("Encrypted password can't be empty. Please try again.");
			}
		}

		try
		{
			var encryptedPassword = _cryptographyService.Decrypt(password);
			WriteResultLog("This is your decrypted password: {0}", encryptedPassword);
		}
		catch (Exception)
		{
			return;
		}
	}

	private void CreateBase64String()
	{
		Console.WriteLine("Enter your string you want to convert to base64");
		var plainText = Console.ReadLine();
		var result = StringUtility.Base64Encode(plainText!);
		WriteResultLog("This is your base64 string: {0}", result);
	}

	private void DecryptBase64String()
	{
		Console.WriteLine("Enter your string you want to decrypt from base64");
		var base64String = Console.ReadLine();
		var result = StringUtility.Base64Decode(base64String!);
		WriteResultLog("This is your decrypt string: {0}", result);
	}

	private void ResolvePath()
	{
		Console.WriteLine("Enter your string you want to resolve");
		Console.WriteLine("Example: /folder1/./folder2/../file.txt");
		var path = Console.ReadLine();
		var result = PathUtility.ResolvePath(path!);
		WriteResultLog("This is your resolved path: {0}", result);
	}

	private FileChecker FileChecker { get; set; }
}
