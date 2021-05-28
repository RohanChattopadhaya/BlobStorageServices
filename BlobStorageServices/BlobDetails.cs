using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStorageServices
{
    public class BlobDetails
    {
        public string BlobName { get; set; }
    }

    public class MetaDataDetails
    {
        public List<MetaData> metaDatas { get; set; }
    }

    public class MetaData
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
