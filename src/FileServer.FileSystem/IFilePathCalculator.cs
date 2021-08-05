using FileServer.FileProvider;

namespace FileServer.FileSystem
{
    public interface IFilePathCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
