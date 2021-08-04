using FileServer.FileProvider;
using System.IO;

namespace FileServer.FileSystem
{
    public class FileSystemBlobOptions : FileProviderSchemeOptions
    {
        public FileSystemBlobOptions()
        {
            
        }

        public string BasePath { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }
    }
}
