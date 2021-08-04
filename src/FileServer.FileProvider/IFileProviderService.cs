using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public interface IFileProviderService
    {
        Task SaveAsync(
            HttpContext context,
            string scheme,
            BlobProviderSaveArgs args
        );


        Task<bool> DeleteAsync(
            HttpContext context,
            string scheme,
            BlobProviderDeleteArgs args
        );


        Task<bool> ExistsAsync(
            HttpContext context,
            string scheme,
            BlobProviderExistsArgs args
        );

        Task<Stream> GetAsync(
            HttpContext context,
            string scheme,
            BlobProviderGetArgs args
        );

        Task<Stream> GetOrNullAsync(
            HttpContext context,
            string scheme,
            BlobProviderGetArgs args
        );

    }
}
