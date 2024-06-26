﻿using Restia.Common.Abstractions.Infrastructure.DI;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Restia.Common.Infrastructure.Security
{
	public class CryptographyService : ICryptographyService
	{
		public CryptographyService(IEnvironmentService envService)
		{
			var rijndael = new RijndaelManaged
			{
				// RijndaelManaged only supports 128, 192 and 256 bit keys.
				Key = Convert.FromBase64String(envService.EncryptionSingleSignOnKey),
				// IV need to be the same size with key
				IV = Convert.FromBase64String(envService.EncryptionSingleSignOnIV)
			};

			this.Encryptor = rijndael.CreateEncryptor();
			this.Decryptor = rijndael.CreateDecryptor();
		}

		public string Encrypt(string source)
		{
			var utf8 = Encoding.UTF8;
			var encryptedSource = Transform(utf8.GetBytes(source), this.Encryptor);
			return Convert.ToBase64String(encryptedSource);
		}

		public string Decrypt(string source)
		{
			Encoding utf8 = Encoding.UTF8;
			byte[] decryptedSource = Transform(Convert.FromBase64String(source), this.Decryptor);
			return utf8.GetString(decryptedSource);
		}

		public string GenerateRandomCryptoValue()
		{
			var numberOfBits = 256; // or 192 or 128, however using a larger bit size renders the encrypted data harder to decipher
			var randomBytes = new byte[numberOfBits / 8]; // 8 bits per byte
			new RNGCryptoServiceProvider().GetBytes(randomBytes);

			return Convert.ToBase64String(randomBytes);
		}

		private byte[] Transform(byte[] source, ICryptoTransform transform)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
				{
					cs.Write(source, 0, source.Length);
				}
				return ms.ToArray();
			}
		}

		private ICryptoTransform Encryptor { get; set; }

		private ICryptoTransform Decryptor { get; set; }
	}
}
