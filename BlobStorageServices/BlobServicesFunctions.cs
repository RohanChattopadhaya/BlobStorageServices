using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlobStorageServices
{
    public class BlobServicesFunctions
    {
        //public BlobServicesFunctions()
        //{
        //    blobServiceClient = new BlobServiceClient(ConnectionString);
        //    containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        //}
        private readonly IBlobService _blobService;
        public BlobServicesFunctions(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [FunctionName("BlobInsert")]
        public async Task<string> BlobInsert(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var data = JsonConvert.DeserializeObject<Grocery>(requestBody);
            var response = await _blobService.Uploadblob(data, "Grocery.Json");

            return response;
        }

        [FunctionName("BlobRead")]
        public async Task<Grocery> BlobRead(
           [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = await _blobService.GetBlobData("Grocery.Json");

            return response;
        }

        [FunctionName("CreateContainer")]
        public async Task<string> CreateContainer(
          [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
          ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = await _blobService.CreateContainer();

            return response;
        }
    }
}
