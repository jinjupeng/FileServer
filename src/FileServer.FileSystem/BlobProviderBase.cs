using FileServer.FileProvider;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileSystem
{
    public abstract class BlobProviderBase : IBlobProvider
    {
        public abstract Task SaveAsync(BlobProviderSaveArgs args);

        public abstract Task<bool> DeleteAsync(BlobProviderDeleteArgs args);

        public abstract Task<bool> ExistsAsync(BlobProviderExistsArgs args);

        public abstract Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);
    }
}
