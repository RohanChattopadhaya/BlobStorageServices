using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStorageServices
{
    public class BlobPropertiesData
    {
        public string AccessTier { get; set; }
        public string BlobType { get; set; }
        public long ContentLength { get; set; }
        public string LastModified { get; set; }
        public string LeaseStatus { get; set; }
        public string ContentType { get; set; }
        public string Shop { get; set; }
        public string ShopID { get; set; }
    }
}
