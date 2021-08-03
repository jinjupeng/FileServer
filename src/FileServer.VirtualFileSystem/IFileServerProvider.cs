using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;

namespace FileServer.VirtualFileSystem
{
    public interface IFileServerProvider
    {
        /// <summary>
        /// Contains a list of FileServer options, a combination of virtual + physical paths we can access at any time
        /// </summary>
        IList<FileServerOptions> FileServerOptionsCollection { get; }

        /// <summary>
        /// Gets the IFileProvider to access a physical location by using its virtual path
        /// </summary>
        IFileProvider GetProvider(string virtualPath);
    }
}
