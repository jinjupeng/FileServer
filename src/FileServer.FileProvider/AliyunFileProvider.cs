using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;

namespace FileServer.FileProvider
{
    public class AliyunFileProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            throw new NotImplementedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
