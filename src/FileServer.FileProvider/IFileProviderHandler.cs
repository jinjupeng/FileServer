using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public interface IFileProviderHandler
    {
        /// <summary>
        /// The handler should initialize anything it needs from the request and scheme here.
        /// </summary>
        /// <param name="scheme">The <see cref="FileProviderScheme"/> scheme.</param>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <returns></returns>
        Task InitializeAsync(FileProviderScheme scheme, HttpContext context);

        /// <summary>
        /// Saves a blob <see cref="Stream"/> to the container.
        /// </summary>
        /// <param name="name">The name of the blob</param>
        /// <param name="stream">A stream for the blob</param>
        /// <param name="overrideExisting">
        /// Set <code>true</code> to override if there is already a blob in the container with the given name.
        /// If set to <code>false</code> (default), throws exception if there is already a blob in the container with the given name.
        /// </param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task SaveAsync(
            BlobProviderSaveArgs args
        );

        /// <summary>
        /// Deletes a blob from the container.
        /// </summary>
        /// <param name="name">The name of the blob</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Returns true if actually deleted the blob.
        /// Returns false if the blob with the given <paramref name="name"/> was not exists.  
        /// </returns>
        Task<bool> DeleteAsync(
            BlobProviderDeleteArgs args
        );

        /// <summary>
        /// Checks if a blob does exists in the container.
        /// </summary>
        /// <param name="name">The name of the blob</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<bool> ExistsAsync(
            BlobProviderExistsArgs args
        );

        /// <summary>
        /// Gets a blob from the container.
        /// It actually gets a <see cref="Stream"/> to read the blob data.
        /// It throws exception if there is no blob with the given <paramref name="name"/>.
        /// Use <see cref="GetOrNullAsync"/> if you want to get <code>null</code> if there is no blob with the given <paramref name="name"/>. 
        /// </summary>
        /// <param name="name">The name of the blob</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="Stream"/> to read the blob data.
        /// </returns>
        Task<Stream> GetAsync(
            BlobProviderGetArgs args
        );

        /// <summary>
        /// Gets a blob from the container, or returns null if there is no blob with the given <paramref name="name"/>.
        /// It actually gets a <see cref="Stream"/> to read the blob data.
        /// </summary>
        /// <param name="name">The name of the blob</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="Stream"/> to read the blob data.
        /// </returns>
        Task<Stream> GetOrNullAsync(
           BlobProviderGetArgs args
        );
    }
}
