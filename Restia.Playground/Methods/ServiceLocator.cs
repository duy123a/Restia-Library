using Microsoft.Extensions.Logging;
using Restia.Common.Abstractions.Services;

namespace Restia.Playground.Methods
{
    public class ServiceLocator
    {
        private readonly ILogger _logger;
        private readonly ICryptographyService _cryptographyService;

        public ServiceLocator(
            ILogger<ServiceLocator> logger,
            ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
            _logger = logger;
        }

        public void GenerateRsaKeyPair()
        {
            var (privateKey, publicKey) = _cryptographyService.GenerateRsaKeyPair();
            _logger.LogInformation("This is your private key: {0}", privateKey);
            _logger.LogInformation("This is your public key: {0}", publicKey);
        }
    }
}
