using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json;
using System;
using System.Text;

namespace QueueStorage
{
    class Program
    {
        private static readonly string connectionString = "";
        private static readonly string queueName = "groceryqueue";
        static void Main(string[] args)
        {
            QueueClient queueClient = new QueueClient(connectionString, queueName);
            queueClient.CreateIfNotExists();
            AddData(queueClient);
            //PeekData(queueClient);
            //ReceiveData(queueClient);
           // DeleteQueue(queueClient);
            Console.ReadKey();
        }

        private static void DeleteQueue(QueueClient queueClient)
        {
           var response = queueClient.Delete();
            if (queueClient.Exists()) {
                if (response.Status.Equals(204))
                {
                    Console.WriteLine("Queue Deleted");
                }
            }
            else
            {
                Console.WriteLine("Queue Not Present");
            }
        }

        private static void ReceiveData(QueueClient queueClient)
        {
            try
            {
                if (queueClient.Exists())
                {
                    QueueMessage[] message = queueClient.ReceiveMessages(2);

                    foreach (var item in message)
                    {
                        var encodeTextData = Convert.FromBase64String(item.Body.ToString());
                        var actualData = JsonConvert.DeserializeObject<Grocery>(Encoding.UTF8.GetString(encodeTextData));
                        Console.WriteLine("id : " + actualData.Id);
                        Console.WriteLine("Item Name : " + actualData.ItemName);
                        Console.WriteLine("Item Price : " + actualData.ItemPrice);
                        queueClient.DeleteMessage(item.MessageId, item.PopReceipt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void PeekData(QueueClient queueClient)
        {
            try
            {
                if (queueClient.Exists())
                {
                    PeekedMessage[] message = queueClient.PeekMessages(2);

                    foreach (var item in message)
                    {
                        var encodeTextData = Convert.FromBase64String(item.Body.ToString());
                        var actualData = JsonConvert.DeserializeObject<Grocery>(Encoding.UTF8.GetString(encodeTextData));
                        Console.WriteLine("id : " + actualData.Id);
                        Console.WriteLine("Item Name : " + actualData.ItemName);
                        Console.WriteLine("Item Price : " + actualData.ItemPrice);
                        // Console.WriteLine(item.Body.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void AddData(QueueClient queueClient)
        {
            try
            {
                if (queueClient.Exists())
                {
                    Grocery grocery = new Grocery()
                    {
                        Id = 146,
                        ItemName = "Mutton",
                        ItemPrice = 750
                    };
                    var data = JsonConvert.SerializeObject(grocery).ToString();
                    var message = Encoding.UTF8.GetBytes(data);
                    var encodedBase64 = Convert.ToBase64String(message);
                    queueClient.SendMessage(encodedBase64);
                    Console.WriteLine("Message Send");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
