using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlobStorageServices
{
    public class BlobService: IBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        BlobServiceClient blobServiceClient;
        BlobContainerClient containerClient;
        BlobClient blobClient;

        public BlobService(string connectionString,string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public async Task<string> CreateContainer()
        {
            blobServiceClient = new BlobServiceClient(_connectionString);
            containerClient = blobServiceClient.CreateBlobContainer(_containerName,PublicAccessType.Blob);
            return Constants.Created;
        }

        public async Task<Grocery> GetBlobData(string blobName)
        {
            containerClient = getDetailsContainer();
            blobClient = containerClient.GetBlobClient(blobName);
            StringBuilder stringBuilder = new StringBuilder();

            if (await blobClient.ExistsAsync()) 
            {
                var blob = blobClient.DownloadAsync().Result;              
                using (var streamReader = new StreamReader(blob.Value.Content))
                {
                    while(!streamReader.EndOfStream) 
                    {
                        string data = await streamReader.ReadToEndAsync();
                        stringBuilder.Append(data);
                    }
                    
                }
            }

            string jsonFormatted = PrettyJson(stringBuilder.ToString());
            var returnData = JsonConvert.DeserializeObject<Grocery>(jsonFormatted);
            return returnData;
        }

        public async Task<string> Uploadblob(Grocery groceryData, string blobName)
        {
            containerClient = getDetailsContainer();
            blobClient = containerClient.GetBlobClient(blobName);
            var serilizedData = JsonConvert.SerializeObject(groceryData, Formatting.Indented);
           
            using (var ms = new MemoryStream())
            {
                LoadStreamWithJson(ms, serilizedData);
                await blobClient.UploadAsync(ms,true);
                return Constants.Uploaded;
            }
        }

        public async Task<string> DeleteBlob(string blobName)
        {
            containerClient = getDetailsContainer();
            blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
            return Constants.Deleted;
        }

        public string PrettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(unPrettyJson);

            return System.Text.Json.JsonSerializer.Serialize(jsonElement, options);
        }

        private BlobContainerClient getDetailsContainer()
        {
            blobServiceClient = new BlobServiceClient(_connectionString);
            containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            return containerClient;
        }

        private static void LoadStreamWithJson(Stream ms, object obj)
        {
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(obj);
            writer.Flush();
            ms.Position = 0;
        }

       
    }
}
