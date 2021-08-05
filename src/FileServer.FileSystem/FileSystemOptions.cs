using System.IO;

namespace FileServer.FileSystem
{
    public class FileSystemOptions
    {
        public FileSystemOptions()
        {
            
        }

        public string BasePath { get; set; }

        public virtual void Validate()
        {
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }
    }
}
