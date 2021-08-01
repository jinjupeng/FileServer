using System.Threading;
using JetBrains.Annotations;

namespace FileServer.FileProvider
{
    public class BlobProviderGetArgs : BlobProviderArgs
    {
        public BlobProviderGetArgs(
            [NotNull] string blobName,
            CancellationToken cancellationToken = default)
            : base(
                blobName,
                cancellationToken)
        {
        }
    }
}
