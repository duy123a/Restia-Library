namespace Restia.Common
{
    public interface ICryptography
    {
        string Encrypt(string source);

        string Decrypt(string source);
    }
}
