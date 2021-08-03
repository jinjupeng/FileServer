using FileServer.FileProvider;

namespace FileServer.Minio
{
    public class DefaultMinioBlobNameCalculator : IMinioBlobNameCalculator
    {
        public virtual string Calculate(BlobProviderArgs args)
        {
            return $"host/{args.BlobName}";
        }
    }
}
