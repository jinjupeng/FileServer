using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public abstract class BlobProviderBase : IBlobProvider
    {
        public abstract Task SaveAsync(BlobProviderSaveArgs args);

        public abstract Task<bool> DeleteAsync(BlobProviderDeleteArgs args);

        public abstract Task<bool> ExistsAsync(BlobProviderExistsArgs args);

        public abstract Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);

        public abstract Task InitializeAsync(FileProviderScheme scheme, HttpContext context);
    }
}
