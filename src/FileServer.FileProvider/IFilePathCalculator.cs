

namespace FileServer.FileProvider
{
    public interface IFilePathCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
