using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Responsible for managing what fileProviderSchemes are supported.
    /// </summary>
    public interface IFileProviderSchemeProvider
    {
        /// <summary>
        /// Returns all currently registered <see cref="FileProviderScheme"/>s.
        /// </summary>
        /// <returns>All currently registered <see cref="FileProviderScheme"/>s.</returns>
        Task<IEnumerable<FileProviderScheme>> GetAllSchemesAsync();

        /// <summary>
        /// Returns the <see cref="FileProviderScheme"/> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the fileProviderSchemes.</param>
        /// <returns>The scheme or null if not found.</returns>
        Task<FileProviderScheme> GetSchemeAsync(string name);

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IFileProviderService.FileProviderAsync(HttpContext, string)"/>.
        /// This is typically specified via <see cref="FileProviderOptions.DefaultFileProviderScheme"/>.
        /// Otherwise, this will fallback to <see cref="FileProviderOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IFileProviderService.FileProviderAsync(HttpContext, string)"/>.</returns>
        Task<FileProviderScheme> GetDefaultFileProviderSchemeAsync();

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="IFileProviderService"/>.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme being removed.</param>
        void RemoveScheme(string name);

        /// <summary>
        /// Returns the schemes in priority order for request handling.
        /// </summary>
        /// <returns>The schemes in priority order for request handling</returns>
        Task<IEnumerable<FileProviderScheme>> GetRequestHandlerSchemesAsync();
    }
}
