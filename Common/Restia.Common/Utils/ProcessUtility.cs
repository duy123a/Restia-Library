using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Restia.Common.Utils;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class ProcessUtility
{
	public static bool ExecWithProcessMutex(Action method)
	{
		var assemblyLocation = Assembly.GetExecutingAssembly().Location;
		if (assemblyLocation == null) return false;
		return ExecWithMutex(GenerateMutexName(assemblyLocation, "Process", true, true), method);
	}

	public static bool ExecWithMutex(string mutexName, Action method)
	{
		// Generate mutex
		using (Mutex mutex = new Mutex(false, mutexName, out _))
		{
			// Flag to check is it running yet
			var hasHandle = false;
			try
			{
				try
				{
					// These mutex security code only work on Window platform
					var mutexSecurity = new MutexSecurity();
					mutexSecurity.AddAccessRule(
						new MutexAccessRule(
							new SecurityIdentifier(WellKnownSidType.WorldSid, null),
							MutexRights.Synchronize | MutexRights.Modify,
							AccessControlType.Allow));
					mutex.SetAccessControl(mutexSecurity);

					hasHandle = mutex.WaitOne(0, false);

					// If the handle could not be obtained
					if (hasHandle == false) return false;
				}
				catch (AbandonedMutexException)
				{
					// Run here if the previous process terminated without releasing the Mutex handle.
					// Even in this case, the handle itself has been acquired, so treat it as an acquisition.
					// Reference: KERNEL32 -> WaitForSingleObjectEx
					// https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitforsingleobjectex?redirectedfrom=MSDN
					hasHandle = true;
					Console.Error.WriteLine("Abandoned Mutex Exception");
				}

				// Process execution
				method.Invoke();

				return true;
			}
			finally
			{
				// Release mutex after using
				if (hasHandle) mutex.ReleaseMutex();
			}
		}
	}

	public static string GenerateMutexName(string filePath, string postfix, bool global, bool lower)
	{
		// String to prepend to Mutex kernel object
		var PREFIX_MUTEX = "restia:";

		// File path part generation
		var filePathEscaped = string.Empty;

		// Hash if over 200 characters
		if (filePath.Length > 200)
		{
			// Lowercase if necessary -> Unicode(UTF-16LE) -> SHA256(uppercase)
			var bytes = Encoding.Unicode.GetBytes(lower ? filePath.ToLower() : filePath);
			filePathEscaped = string.Join(string.Empty, SHA256.HashData(bytes).Select(item => item.ToString("X2")));
		}
		else
		{
			// Lower case if necessary->backslash becomes forward slash
			filePathEscaped = (lower
				? filePath.Replace(@"\", "/").ToLower()
				: filePath.Replace(@"\", "/"));
		}

		// Mutex name generation
		return string.Format(@"{0}\{1}:{2}.{3}",
			(global ? @"Global" : @"Local"),    // Kernel namespace Prefix
			PREFIX_MUTEX,    // conflict prevention
			filePathEscaped,
			postfix);
	}
}
