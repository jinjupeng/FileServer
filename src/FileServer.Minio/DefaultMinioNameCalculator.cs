using FileServer.FileProvider;

namespace FileServer.Minio
{
    public class DefaultMinioNameCalculator : IFilePathCalculator
    {
        public virtual string Calculate(BlobProviderArgs args)
        {
            return $"host/{args.BlobName}";
        }
    }
}
