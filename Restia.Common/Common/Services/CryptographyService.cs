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
        /// <summary>
        /// Generates a new RSA key pair with a 2048-bit key size,
        /// exports both the private and public keys in PEM format,
        /// writes them to local files, and returns them as strings.
        /// </summary>
        /// <returns>
        /// A tuple containing the private key and public key in PEM format.
        /// </returns>
        public (string privateKey, string publicKey) GenerateRsaKeyPair()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 2048;

                var privateKey = ExportPrivateKeyPem(rsa);
                var publicKey = ExportPublicKeyPem(rsa);

                // Export private and public key (PEM)
                File.WriteAllText("private.key", privateKey);
                File.WriteAllText("public.key", publicKey);

                return (privateKey, publicKey);
            }
        }

        /// <summary>
        /// Generates a PKCE code challenge from a code verifier string
        /// by computing its SHA256 hash and encoding the result in Base64 URL format.
        /// </summary>
        /// <param name="codeVerifier">The original code verifier string.</param>
        /// <returns>The Base64 URL-encoded SHA256 hash of the code verifier.</returns>
        public string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.ASCII.GetBytes(codeVerifier);
                var hash = sha256.ComputeHash(bytes);
                return ObjectUtility.Base64UrlEncode(hash);
            }
        }

        private string ExportPrivateKeyPem(RSA rsa)
        {
            var parameters = rsa.ExportParameters(true);
            var privateKeyBytes = EncodePrivateKey(parameters);

            var base64 = Convert.ToBase64String(privateKeyBytes);
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN RSA PRIVATE KEY-----");
            for (var i = 0; i < base64.Length; i += 64)
            {
                sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
            }
            sb.AppendLine("-----END RSA PRIVATE KEY-----");
            return sb.ToString();
        }

        private string ExportPublicKeyPem(RSA rsa)
        {
            var parameters = rsa.ExportParameters(false);
            var publicKeyBytes = EncodePublicKey(parameters);

            var base64 = Convert.ToBase64String(publicKeyBytes);
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN PUBLIC KEY-----");
            for (var i = 0; i < base64.Length; i += 64)
            {
                sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
            }
            sb.AppendLine("-----END PUBLIC KEY-----");
            return sb.ToString();
        }

        // Encode RSA Private Key in PKCS#1 format (ASN.1 DER)
        private byte[] EncodePrivateKey(RSAParameters parameters)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                // Write ASN.1 sequence
                WriteAsn1Sequence(writer, () =>
                {
                    WriteAsn1Integer(writer, new byte[] { 0x00 }); // version
                    WriteAsn1Integer(writer, parameters.Modulus);
                    WriteAsn1Integer(writer, parameters.Exponent);
                    WriteAsn1Integer(writer, parameters.D);
                    WriteAsn1Integer(writer, parameters.P);
                    WriteAsn1Integer(writer, parameters.Q);
                    WriteAsn1Integer(writer, parameters.DP);
                    WriteAsn1Integer(writer, parameters.DQ);
                    WriteAsn1Integer(writer, parameters.InverseQ);
                });

                return stream.ToArray();
            }
        }

        // Encode RSA Public Key in X.509 SubjectPublicKeyInfo format
        private byte[] EncodePublicKey(RSAParameters parameters)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                // SubjectPublicKeyInfo sequence
                WriteAsn1Sequence(writer, () =>
                {
                    // AlgorithmIdentifier sequence
                    WriteAsn1Sequence(writer, () =>
                    {
                        // rsaEncryption OID: 1.2.840.113549.1.1.1
                        WriteAsn1ObjectIdentifier(writer, new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 });
                        WriteAsn1Null(writer);
                    });

                    // BIT STRING of RSAPublicKey
                    var publicKey = EncodeRSAPublicKey(parameters);
                    WriteAsn1BitString(writer, publicKey);
                });

                return stream.ToArray();
            }
        }

        private byte[] EncodeRSAPublicKey(RSAParameters parameters)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                WriteAsn1Sequence(writer, () =>
                {
                    WriteAsn1Integer(writer, parameters.Modulus);
                    WriteAsn1Integer(writer, parameters.Exponent);
                });

                return stream.ToArray();
            }
        }

        #region ASN.1 helper methods

        private void WriteAsn1Sequence(BinaryWriter writer, Action writeContent)
        {
            using (var contentStream = new MemoryStream())
            using (var contentWriter = new BinaryWriter(contentStream))
            {
                writeContent();

                var contentBytes = contentStream.ToArray();
                writer.Write((byte)0x30); // SEQUENCE
                WriteAsnLength(writer, contentBytes.Length);
                writer.Write(contentBytes);
            }
        }

        private void WriteAsn1Integer(BinaryWriter writer, byte[] value)
        {
            writer.Write((byte)0x02); // INTEGER

            int prefixZeros = 0;
            while (prefixZeros < value.Length && value[prefixZeros] == 0x00)
            {
                prefixZeros++;
            }

            if (prefixZeros == value.Length)
            {
                // Integer value is 0
                WriteAsnLength(writer, 1);
                writer.Write((byte)0x00);
                return;
            }

            // If the highest bit is set, prepend 0x00
            bool needsLeadingZero = (value[prefixZeros] & 0x80) != 0;

            int length = value.Length - prefixZeros + (needsLeadingZero ? 1 : 0);
            WriteAsnLength(writer, length);

            if (needsLeadingZero)
            {
                writer.Write((byte)0x00);
            }

            for (int i = prefixZeros; i < value.Length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private void WriteAsnLength(BinaryWriter writer, int length)
        {
            if (length < 0x80)
            {
                writer.Write((byte)length);
            }
            else
            {
                int temp = length;
                int byteCount = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    byteCount++;
                }

                writer.Write((byte)(0x80 | byteCount));

                for (int i = byteCount - 1; i >= 0; i--)
                {
                    writer.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private void WriteAsn1ObjectIdentifier(BinaryWriter writer, byte[] oid)
        {
            writer.Write((byte)0x06); // OBJECT IDENTIFIER
            WriteAsnLength(writer, oid.Length);
            writer.Write(oid);
        }

        private void WriteAsn1Null(BinaryWriter writer)
        {
            writer.Write((byte)0x05); // NULL
            writer.Write((byte)0x00);
        }

        private void WriteAsn1BitString(BinaryWriter writer, byte[] bytes)
        {
            writer.Write((byte)0x03); // BIT STRING
            WriteAsnLength(writer, bytes.Length + 1);
            writer.Write((byte)0x00); // Number of unused bits
            writer.Write(bytes);
        }

        #endregion
    }
}
