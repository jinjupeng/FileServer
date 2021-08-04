using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Used to determine if a handler wants to participate in request processing.
    /// </summary>
    public interface IFileProviderRequestHandler : IFileProviderHandler
    {
        /// <summary>
        /// Returns true if request processing should stop.
        /// </summary>
        /// <returns><code>true</code> if request processing should stop.</returns>
        Task<bool> HandleRequestAsync();
    }
}
