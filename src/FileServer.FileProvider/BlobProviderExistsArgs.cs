using System.Threading;
using JetBrains.Annotations;

namespace FileServer.FileProvider
{
    public class BlobProviderExistsArgs : BlobProviderArgs
    {
        public BlobProviderExistsArgs(
            [NotNull] string blobName,
            CancellationToken cancellationToken = default)
        : base(
            blobName,
            cancellationToken)
        {
        }
    }
}
