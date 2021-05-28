using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace MultipleInputQueueTrigger
{
    public class MultipleInput
    {
        //if name of the blob in queue it will return that blob details 
        [FunctionName("MultipleInputFunction")]
        public static void Run([QueueTrigger("groceryqueue", Connection = "ConnectionString")] string myQueueItem,
            [Blob("grocerycontainer/{queueTrigger}", FileAccess.Read, Connection = "ConnectionString")] Stream blob,
            ILogger log)
        {      
           
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            StreamReader reader = new StreamReader(blob);
            log.LogInformation(reader.ReadToEnd());

        }

        // To write data to table/blob
        //[FunctionName("MultipleInputFunctionCheck")]
        //public async Task Run([QueueTrigger("groceryqueue", Connection = "ConnectionString")] JObject myQueueItem,
        //[Table("Grocery", Connection = "ConnectionString")] IAsyncCollector<Grocery> input,
        //[Blob("grocerycontainer/DetailsGrocery.json", FileAccess.Read | FileAccess.Write, Connection = "ConnectionString")] TextWriter textWriter,
        //ILogger log)
        //{

        //    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        //    Grocery grocery = new Grocery()
        //    {
        //        PartitionKey = myQueueItem["Id"].ToString(),
        //        RowKey = myQueueItem["ItemName"].ToString(),
        //        Id = Convert.ToInt32(myQueueItem["Id"]),
        //        ItemName = myQueueItem["ItemName"].ToString(),
        //        ItemPrice = Convert.ToDouble(myQueueItem["ItemPrice"])
        //    };
        //    await input.AddAsync(grocery);

        //    //text writer to write inside blob
        //    textWriter.Write(Convert.ToInt32(myQueueItem["Id"]));
        //    textWriter.Write(myQueueItem["ItemName"].ToString());
        //    textWriter.Write(Convert.ToDecimal(myQueueItem["ItemPrice"]));
        //}
    }
}
