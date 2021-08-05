using FileServer.FileProvider;

namespace FileServer.Minio
{
    public class DefaultMinioNameCalculator : IMinioNameCalculator
    {
        public virtual string Calculate(BlobProviderArgs args)
        {
            return $"host/{args.BlobName}";
        }
    }
}
