using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public interface IBlobProvider
    {
        Task SaveAsync(BlobProviderSaveArgs args);

        Task<bool> DeleteAsync(BlobProviderDeleteArgs args);

        Task<bool> ExistsAsync(BlobProviderExistsArgs args);

        Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);
    }
}
