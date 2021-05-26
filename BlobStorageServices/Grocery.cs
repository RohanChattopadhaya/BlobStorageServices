using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStorageServices
{
    public class Grocery
    {
        public int id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public List<AllItems> AlllItems { get; set; }
    }

    public class AllItems
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int NoOfItem { get; set; }
        public decimal ItemPrice { get; set; }
    }
}
