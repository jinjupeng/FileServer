using FileServer.FileProvider;

namespace FileServer.Minio
{
    public interface IMinioNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
