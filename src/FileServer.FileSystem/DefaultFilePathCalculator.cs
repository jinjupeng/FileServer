using FileServer.FileProvider;
using Microsoft.Extensions.Options;
using System.IO;

namespace FileServer.FileSystem
{
    public class DefaultFilePathCalculator : IFilePathCalculator
    {
        private readonly FileSystemOptions fileSystemOptions;

        public DefaultFilePathCalculator(IOptions<FileSystemOptions> options)
        {
            fileSystemOptions = options?.Value;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var blobPath = fileSystemOptions.BasePath;

            blobPath = Path.Combine(blobPath, FileSystemDefaults.FileProviderScheme, args.BlobName);

            return blobPath;
        }
    }
}
