using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace QueueStorageAzureFunction
{
    public static class QueueStorageFunction
    {
        
        [FunctionName("QueueStorageFunction")]
        [return: Table("Grocery", Connection = "ConnectionString")]
        public static Grocery Run([QueueTrigger("groceryqueue", Connection = "ConnectionString")]JObject myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            Grocery grocery = new Grocery()
            {
                PartitionKey = myQueueItem["Id"].ToString(),
                RowKey = myQueueItem["ItemName"].ToString(),
                Id = Convert.ToInt32(myQueueItem["Id"]),
                ItemName = myQueueItem["ItemName"].ToString(),
                ItemPrice = Convert.ToDouble(myQueueItem["ItemPrice"])
            };

            return grocery;
        }
    }
}
