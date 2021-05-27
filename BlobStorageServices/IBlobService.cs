using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlobStorageServices
{
    public interface IBlobService
    {
        Task<string> CreateContainer();
        Task<string> Uploadblob(Grocery groceryData,string blobName);
        Task<Grocery> GetBlobData(string blobName);
        Task<string> DeleteBlob(string blobName);
        Task<List<BlobDetails>> GetBlobs();
        Task<Grocery> GetBlobDataWithSAS(string blobName);
        Task<BlobPropertiesData> GetBlobProperties(string blobName);

    }
}
