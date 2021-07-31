using Microsoft.AspNetCore.Builder;
using System.IO;

namespace FileServer.OSS
{
    public class OSSOptions : FileServerOptions
    {
        public OSSOptions() { }

        /// <summary>
        /// 文件上传路径
        /// </summary>
        public string UploadPath { 
            get 
            {
                return uploadPath;
            }
            set
            {
                uploadPath = value;
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
            }
        }

        /// <summary>
        /// 文件存储绝对路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 文件访问url
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 文件信息
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// 文件存储处理器
        /// </summary>
        public IOSSProvider OSSProvider { get; set; }

        public int ProviderType { get; set; } = 0;

        private string uploadPath;
    }
}
