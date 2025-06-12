using Restia.Common.Common.Interfaces;

namespace Restia.Common.Abstractions.Services
{
    public interface ICryptographyService : IScopedService
    {
        (string privateKey, string publicKey) GenerateRsaKeyPair();

        string GenerateCodeChallenge(string codeVerifier);

        string HashPassword(string password);

        bool VerifyPassword(string password, string savedHash);
    }
}