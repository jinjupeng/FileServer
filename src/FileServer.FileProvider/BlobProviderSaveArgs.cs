using System.IO;
using System.Threading;
using FileServer.Common.Helper;
using JetBrains.Annotations;

namespace FileServer.FileProvider
{
    public class BlobProviderSaveArgs : BlobProviderArgs
    {
        [NotNull]
        public Stream BlobStream { get; }

        public bool OverrideExisting { get; }

        public BlobProviderSaveArgs(
            [NotNull] string blobName,
            [NotNull] Stream blobStream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
            : base(
                blobName,
                cancellationToken)
        {
            BlobStream = ObjHelper.NotNull(blobStream, nameof(blobStream));
            OverrideExisting = overrideExisting;
        }
    }
}
