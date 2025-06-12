using Microsoft.Extensions.Logging;
using Restia.Common.Abstractions.Services;

namespace Restia.Playground.Methods
{
    public class ServiceLocator
    {
        private readonly ILogger _logger;
        private readonly ICryptographyService _cryptographyService;
        private dynamic _temp;

        public ServiceLocator(
            ILogger<ServiceLocator> logger,
            ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
            _logger = logger;
            _temp = string.Empty;
        }

        public void GenerateRsaKeyPair()
        {
            var (privateKey, publicKey) = _cryptographyService.GenerateRsaKeyPair();
            _logger.LogInformation("This is your private key: {0}", privateKey);
            _logger.LogInformation("This is your public key: {0}", publicKey);
        }

        public void HashPassword()
        {
            Console.WriteLine("Please enter your password: ");
            var password = Console.ReadLine();
            var result = _cryptographyService.HashPassword(password);
            _logger.LogInformation("This is your hash password: {0}", result);
            _temp = result;
            _logger.LogInformation("The result is cached in temp");
        }

        public void VerifyPassword()
        {
            Console.WriteLine("Please enter your password: ");
            var password = Console.ReadLine();
            var result = _cryptographyService.VerifyPassword(password, _temp);
            if (result)
            {
                _logger.LogInformation("This is a correct password");
            }
            else
            {
                _logger.LogInformation("This is a incorrect password");
            }
        }
    }
}
