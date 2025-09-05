using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Options;
using MovilidadSostenible_YAMAHA.Models;
using Newtonsoft.Json;
using System.Security;

namespace MovilidadSostenible_YAMAHA.Data
{
    public class AmazonSecret
    {
        private readonly SecretString _settings;
        public AmazonSecret(IOptions<SecretString> options)
        {
            _settings = options.Value;
        }
        public async Task<SecretString> GetSecretAsync()
        {
            var envUsername = Environment.GetEnvironmentVariable("DB_USER");
            var envPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var envDatabase = Environment.GetEnvironmentVariable("DB_NAME");
            var envServername = Environment.GetEnvironmentVariable("DB_HOST");

            if (!string.IsNullOrEmpty(envUsername) && 
                !string.IsNullOrEmpty(envPassword) && 
                !string.IsNullOrEmpty(envDatabase) && 
                !string.IsNullOrEmpty(envServername))
            {
                return new SecretString() 
                { 
                    username = envUsername, 
                    password = envPassword, 
                    database = envDatabase, 
                    servername = envServername 
                };
            }

            // Si no hay variables de entorno, continuar con el método original
            // En este ejemplo, devuelve las credenciales hardcoded, pero podría
            // implementarse la lógica para obtener desde AWS Secrets Manager
            return new SecretString() 
            { 
                username = "", 
                password = "", 
                database = "", 
                servername = "" 
            };
        }
    }
}
