using System.Threading;
using JetBrains.Annotations;

namespace FileServer.FileProvider
{
    public class BlobProviderDeleteArgs : BlobProviderArgs
    {
        public BlobProviderDeleteArgs(
            [NotNull] string blobName,
            CancellationToken cancellationToken = default)
            : base(
                blobName,
                cancellationToken)
        {
        }
    }
}
