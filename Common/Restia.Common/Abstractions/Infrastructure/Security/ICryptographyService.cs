namespace Restia.Common.Infrastructure.Security
{
	public interface ICryptographyService
	{
		string Encrypt(string source);

		string Decrypt(string source);

		string GenerateRandomCryptoValue();
	}
}
