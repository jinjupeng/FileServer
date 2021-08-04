using FileServer.FileProvider;
using Microsoft.Extensions.Options;
using System.IO;

namespace FileServer.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator
    {
        private readonly FileSystemBlobOptions fileSystemOptions;

        public DefaultBlobFilePathCalculator(IOptionsMonitor<FileSystemBlobOptions> options)
        {
            fileSystemOptions = options.Get(FileSystemBlobDefaults.FileProviderScheme);
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var blobPath = fileSystemOptions.BasePath;

            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}
