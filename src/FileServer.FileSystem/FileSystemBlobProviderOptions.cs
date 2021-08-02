using System.IO;

namespace FileServer.FileSystem
{
    public class FileSystemBlobProviderOptions
    {
        private string basePath;

        public FileSystemBlobProviderOptions() { }

        /// <summary>
        /// 文件上传路径
        /// </summary>
        public string BasePath
        {
            get
            {
                return basePath;
            }
            set
            {
                basePath = value;
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
            }
        }
    }
}
