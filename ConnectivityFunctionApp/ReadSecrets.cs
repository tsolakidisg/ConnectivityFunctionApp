using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace ConnectivityFunctionApp
{
    public static class ReadSecrets
    {
        [FunctionName("ReadSecrets")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var secret = Environment.GetEnvironmentVariable("SecretFromKeyVault", EnvironmentVariableTarget.Process);

            log.LogInformation($"Secret: { secret}");

            var cert = Environment.GetEnvironmentVariable("CertificateFromKeyVault", EnvironmentVariableTarget.Process);
            byte[] certBytes = Convert.FromBase64String(cert);
            var certificate = new X509Certificate2(certBytes, String.Empty,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            var signingCredentials = new X509SigningCredentials(certificate, "RS256");

            return new OkObjectResult(signingCredentials);
        }
    }
}
