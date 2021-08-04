using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Provides the appropriate IFileProviderHandler instance for the fileProviderScheme and request.
    /// </summary>
    public interface IFileProviderHandlerProvider
    {
        /// <summary>
        /// Returns the handler instance that will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="fileProviderScheme">The name of the fileProvider scheme being handled.</param>
        /// <returns>The handler instance.</returns>
        Task<IFileProviderHandler> GetHandlerAsync(HttpContext context, string fileProviderScheme);
    }
}
