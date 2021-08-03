using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public interface IBlobProvider
    {
        /// <summary>
        /// The handler should initialize anything it needs from the request and scheme here.
        /// </summary>
        /// <param name="scheme">The <see cref="FileProviderScheme"/> scheme.</param>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <returns></returns>
        Task InitializeAsync(FileProviderScheme scheme, HttpContext context);

        Task SaveAsync(BlobProviderSaveArgs args);

        Task<bool> DeleteAsync(BlobProviderDeleteArgs args);

        Task<bool> ExistsAsync(BlobProviderExistsArgs args);

        Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);
    }
}
