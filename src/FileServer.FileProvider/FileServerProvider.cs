using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Implements IFileServerProvider in a very simple way, for demonstration only
    /// </summary>
    public class FileServerProvider : IFileServerProvider
    {
        public FileServerProvider(IList<FileServerOptions> fileServerOptions)
        {
            FileServerOptionsCollection = fileServerOptions;
        }

        public IList<FileServerOptions> FileServerOptionsCollection { get; }

        public IFileProvider GetProvider(string virtualPath)
        {
            var options = FileServerOptionsCollection.FirstOrDefault(e => e.RequestPath == virtualPath);
            if (options != null)
                return options.FileProvider;

            throw new FileNotFoundException($"virtual path {virtualPath} is not registered in the fileserver provider");
        }
    }
}
