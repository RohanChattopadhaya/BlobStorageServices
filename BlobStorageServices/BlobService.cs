using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BlobService> logger;

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
            try
            {
                containerClient = getDetailsContainer();
                blobClient = containerClient.GetBlobClient(blobName);
                StringBuilder stringBuilder = new StringBuilder();

                if (await blobClient.ExistsAsync())
                {
                    var blob = blobClient.DownloadAsync().Result;

                    using (var streamReader = new StreamReader(blob.Value.Content))
                    {
                        while (!streamReader.EndOfStream)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> Uploadblob(Grocery groceryData, string blobName)
        {
            try
            {
                containerClient = getDetailsContainer();
                blobClient = containerClient.GetBlobClient(blobName);

                //Aquire lease
                //BlobLeaseClient blobLeaseClient = blobClient.GetBlobLeaseClient();
                //BlobLease blobLease = blobLeaseClient.Acquire(TimeSpan.FromSeconds(25));
                //BlobUploadOptions options = new BlobUploadOptions()
                //{
                //    Conditions = new BlobRequestConditions()
                //    {
                //        LeaseId = blobLease.LeaseId
                //    }                
                //};

                var serilizedData = JsonConvert.SerializeObject(groceryData, Formatting.Indented);

                using (var ms = new MemoryStream())
                {
                    LoadStreamWithJson(ms, serilizedData);
                    await blobClient.UploadAsync(ms, true);
                    // await blobClient.UploadAsync(ms, options);  // blob upload option with lease
                    //blobLeaseClient.Release();
                    return Constants.Uploaded;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> DeleteBlob(string blobName)
        {
            try
            {
                containerClient = getDetailsContainer();
                blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
                return Constants.Deleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BlobDetails>> GetBlobs()
        {
            try
            {
                containerClient = getDetailsContainer();
                List<BlobDetails> bloblist = new List<BlobDetails>();
                BlobDetails blobDetails = new BlobDetails();
                foreach (BlobItem item in containerClient.GetBlobs())
                {
                    blobDetails.BlobName = item.Name;
                    bloblist.Add(blobDetails);
                }
                return bloblist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Grocery> GetBlobDataWithSAS(string blobName)
        {
            try
            {
                Uri blobUri = GetSAS(blobName);
                blobClient = new BlobClient(blobUri);
                StringBuilder stringBuilder = new StringBuilder();
                if (await blobClient.ExistsAsync())
                {
                    var blob = blobClient.DownloadAsync().Result;
                    using (var streamReader = new StreamReader(blob.Value.Content))
                    {
                        while (!streamReader.EndOfStream)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BlobPropertiesData> GetBlobProperties(string blobName)
        {
            try
            {
                var propertiesData = new BlobPropertiesData();
                containerClient = getDetailsContainer();
                blobClient = containerClient.GetBlobClient(blobName);

                BlobProperties blobProperties = blobClient.GetProperties();

                propertiesData.AccessTier = blobProperties.AccessTier;
                propertiesData.BlobType = blobProperties.BlobType.ToString();
                propertiesData.ContentLength = blobProperties.ContentLength;
                propertiesData.LastModified = blobProperties.LastModified.ToString();
                propertiesData.LeaseStatus = blobProperties.LeaseStatus.ToString();
                propertiesData.ContentType = blobProperties.ContentType;

                IDictionary<string, string> keyValuePairs = blobProperties.Metadata;
                propertiesData.Shop = keyValuePairs["Shop"];
                propertiesData.ShopID = keyValuePairs["ShopID"];

                //foreach (KeyValuePair<string,string> item in keyValuePairs)
                //{
                //    logger.LogInformation("Items keys" + item.Key);
                //    logger.LogInformation("Items Value" + item.Value);               
                //}

                return propertiesData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> SetMetaData(string blobName, List<MetaData> data)
        {
            containerClient = getDetailsContainer();
            blobClient = containerClient.GetBlobClient(blobName);

            //add properties
            BlobProperties blobProperties = blobClient.GetProperties();
            IDictionary<string, string> metaDeta = blobProperties.Metadata;
            foreach (var item in data)
            {
                metaDeta.Add(item.key, item.value);
            }          
            blobClient.SetMetadata(metaDeta);

            return Constants.MetaDataCreated;
        }

        private Uri GetSAS(string blobName)
        {
            containerClient = getDetailsContainer();
            blobClient = containerClient.GetBlobClient(blobName);
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _containerName,
                BlobName = blobName,
                ContentType = "application/json",
                Resource = "b"
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.List);
            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(2);

            return blobClient.GenerateSasUri(sasBuilder);
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

        //take data in memory stream and change
        //public async Task<string> addMoreData(string blobName)
        //{
        //    containerClient = getDetailsContainer();
        //    blobClient = containerClient.GetBlobClient(blobName);

        //    MemoryStream memory = new MemoryStream();
        //    blobClient.DownloadTo(memory);

        //    memory.Position = 0;
        //    StreamReader streamReader = new StreamReader(memory);

        //    var data = streamReader.ReadToEnd();

        //    StreamWriter streamWriter = new StreamWriter(memory);
        //    streamWriter.Write("");
        //    streamWriter.Flush();
        //    memory.Position = 0;

        //    await blobClient.UploadAsync(memory, true);

        //    return Constants.Uploaded;
                     
        //}
    }
}
