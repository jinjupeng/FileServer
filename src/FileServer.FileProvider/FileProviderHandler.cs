using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public abstract class FileProviderHandler : IFileProviderHandler
    {
        public abstract Task<bool> DeleteAsync(BlobProviderDeleteArgs args);
        public abstract Task<bool> ExistsAsync(BlobProviderExistsArgs args);
        public abstract Task<Stream> GetAsync(BlobProviderGetArgs args);
        public abstract Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);
        public abstract Task SaveAsync(BlobProviderSaveArgs args);
    }
}
