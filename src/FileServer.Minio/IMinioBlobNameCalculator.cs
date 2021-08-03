using FileServer.FileProvider;

namespace FileServer.Minio
{
    public interface IMinioBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
