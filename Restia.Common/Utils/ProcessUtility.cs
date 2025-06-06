using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Restia.Common.Utils
{
    public static class ProcessUtility
    {
        public static bool ExecWithProcessMutex(Action method)
        {
            var location = Assembly.GetEntryAssembly()?.Location;
            if (string.IsNullOrEmpty(location)) return false;

            var mutexName = GenerateMutexName(location, "Process", global: true, toLower: true);
            return ExecWithMutex(mutexName, method);
        }

        public static bool ExecWithMutex(string mutexName, Action method)
        {
            using (var mutex = new Mutex(initiallyOwned: false, mutexName, out _))
            {
                var hasSignal = false;

                try
                {
                    try
                    {
                        hasSignal = mutex.WaitOne(0, false);
                        if (!hasSignal) return false;
                    }
                    catch (AbandonedMutexException)
                    {
                        hasSignal = true;
                        Console.Error.WriteLine($"⚠ Abandoned mutex detected: {mutexName}");
                    }

                    method();
                    return true;
                }
                finally
                {
                    if (hasSignal)
                    {
                        try { mutex.ReleaseMutex(); }
                        catch (ApplicationException ex)
                        {
                            Console.Error.WriteLine($"⚠ Failed to release mutex: {ex.Message}");
                        }
                    }
                }
            }
        }

        public static string GenerateMutexName(string filePath, string postfix, bool global, bool toLower)
        {
            const string prefix = "restia:";
            string sanitizedPath;

            if (filePath.Length > 200)
            {
                // Use SHA256
                var bytes = Encoding.Unicode.GetBytes(toLower ? filePath.ToLower() : filePath);
                using (var sha256 = SHA256.Create())
                {
                    sanitizedPath = BitConverter.ToString(sha256.ComputeHash(bytes)).Replace("-", "");
                }
            }
            else
            {
                sanitizedPath = (toLower ? filePath.ToLowerInvariant() : filePath)
                    .Replace('\\', '/'); // Normalize for safety
            }

            var name = $"{prefix}{sanitizedPath}.{postfix}";
            return $@"{(global ? "Global" : "Local")}\{name}";
        }
    }
}
