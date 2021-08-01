using System.Threading;
using FileServer.Common.Helper;
using JetBrains.Annotations;

namespace FileServer.FileProvider
{
    public abstract class BlobProviderArgs 
    {
        [NotNull]
        public string BlobName { get; }

        public CancellationToken CancellationToken { get; }

        protected BlobProviderArgs(

            [NotNull] string blobName,

            CancellationToken cancellationToken = default)
        {
            BlobName = ObjHelper.NotNullOrWhiteSpace(blobName, nameof(blobName));
            CancellationToken = cancellationToken;
        }
    }
}
