using FileServer.FileProvider;

namespace FileServer.FileSystem
{
    public interface IBlobFilePathCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
