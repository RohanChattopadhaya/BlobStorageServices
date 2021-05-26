using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(BlobStorageServices.Startup))]
namespace BlobStorageServices
{
    public class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureSettings();
            builder.Services.AddSingleton<IBlobService>(InitializeBlobClientInstanceAsync(_configuration.GetSection("BlobStorage")).GetAwaiter().GetResult());
        }

        private void ConfigureSettings()
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            _configuration = config;
        }

        private static async Task<BlobService> InitializeBlobClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string connectionString = configurationSection.GetSection("ConnectionString").Value;
            
            BlobService blobService = new BlobService(connectionString, containerName);
            return blobService;
        }
    }
}
