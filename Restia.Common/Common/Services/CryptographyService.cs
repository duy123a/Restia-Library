using Restia.Common.Abstractions.Services;
using Restia.Common.Utils;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Restia.Common.Common.Services
{
    public class CryptographyService : ICryptographyService
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int PBKDF2_ITERATIONS = 100_000;

        /// <summary>
        /// Generates a new RSA key pair with a 2048-bit key size and returns them in PEM format.
        /// </summary>
        public (string privateKey, string publicKey) GenerateRsaKeyPair()
        {
            using var rsa = RSA.Create();
            rsa.KeySize = 2048;

            var privateKey = ExportPrivateKeyPem(rsa);
            var publicKey = ExportPublicKeyPem(rsa);

            return (privateKey, publicKey);
        }

        /// <summary>
        /// Writes RSA private and public key to file in PEM format.
        /// </summary>
        public void WriteRsaKeysToFiles(string privateKey, string publicKey, string privatePath = "private.key", string publicPath = "public.key")
        {
            File.WriteAllText(privatePath, privateKey);
            File.WriteAllText(publicPath, publicKey);
        }

        /// <summary>
        /// Generates a PKCE code challenge from a code verifier string.
        /// </summary>
        public string GenerateCodeChallenge(string codeVerifier)
        {
            if (string.IsNullOrWhiteSpace(codeVerifier))
                throw new ArgumentException("Code verifier must not be null or empty", nameof(codeVerifier));

            using var sha256 = SHA256.Create();
            var bytes = Encoding.ASCII.GetBytes(codeVerifier);
            var hash = sha256.ComputeHash(bytes);
            return ObjectUtility.Base64UrlEncode(hash);
        }

        /// <summary>
        /// Hashes a plain-text password using PBKDF2 with SHA256 and a unique salt.
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password must not be null or empty", nameof(password));

            var salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, PBKDF2_ITERATIONS, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            var result = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Verifies a password against a stored Base64-encoded PBKDF2 hash.
        /// </summary>
        public bool VerifyPassword(string password, string savedHash)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password must not be null or empty", nameof(password));
            if (string.IsNullOrWhiteSpace(savedHash))
                return false;

            byte[] savedHashBytes;
            try
            {
                savedHashBytes = Convert.FromBase64String(savedHash);
            }
            catch
            {
                return false;
            }

            if (savedHashBytes.Length != SaltSize + HashSize)
                return false;

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(savedHashBytes, 0, salt, 0, SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, PBKDF2_ITERATIONS, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            int diff = 0;
            for (int i = 0; i < HashSize; i++)
                diff |= savedHashBytes[i + SaltSize] ^ hash[i];

            return diff == 0;
        }

        private string ExportPrivateKeyPem(RSA rsa)
        {
            var keyBytes = rsa.ExportPkcs8PrivateKey();
            var base64 = Convert.ToBase64String(keyBytes, Base64FormattingOptions.InsertLineBreaks);
            return $"-----BEGIN PRIVATE KEY-----\n{base64}\n-----END PRIVATE KEY-----";
        }

        private string ExportPublicKeyPem(RSA rsa)
        {
            var keyBytes = rsa.ExportSubjectPublicKeyInfo();
            var base64 = Convert.ToBase64String(keyBytes, Base64FormattingOptions.InsertLineBreaks);
            return $"-----BEGIN PUBLIC KEY-----\n{base64}\n-----END PUBLIC KEY-----";
        }
    }
}
