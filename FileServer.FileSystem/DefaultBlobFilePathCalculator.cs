using FileServer.FileProvider;
using Microsoft.Extensions.Options;
using System.IO;

namespace FileServer.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator
    {
        private readonly FileSystemBlobProviderOptions fileSystemOptions;

        public DefaultBlobFilePathCalculator(IOptions<FileSystemBlobProviderOptions> options)
        {
            fileSystemOptions = options?.Value;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var blobPath = fileSystemOptions.BasePath;

            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}
