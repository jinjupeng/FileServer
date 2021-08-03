using System.IO;

namespace FileServer.FileSystem
{
    public class FileSystemBlobOptions
    {
        private string basePath;

        public FileSystemBlobOptions() { }

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
