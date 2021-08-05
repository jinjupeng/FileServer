﻿using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public interface IFileProviderHandler
    {
        Task SaveAsync(BlobProviderSaveArgs args);

        Task<bool> DeleteAsync(BlobProviderDeleteArgs args);

        Task<bool> ExistsAsync(BlobProviderExistsArgs args);

        Task<Stream> GetAsync(BlobProviderGetArgs args);

        Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);
    }
}
