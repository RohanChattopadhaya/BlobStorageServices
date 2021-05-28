using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueueStorageAzureFunction
{
    public class Grocery: TableEntity
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public double ItemPrice { get; set; }
    }
}
