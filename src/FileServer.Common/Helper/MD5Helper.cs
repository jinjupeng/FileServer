using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileServer.Common.Helper
{
    public class MD5Helper
    {
        /// <summary>
        /// 使用MD5计算哈希值
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns></returns>
        public static string GetMD5Hash(string filePath)
        {
            var hashMd5 = string.Empty;
            if (File.Exists(filePath))
            {
                // 使用using，自动关闭文件流
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var md5 = MD5.Create();
                    var buffer = md5.ComputeHash(fs);
                    md5.Clear();
                    // 将字节数组转换成十六进制的字符串形式
                    var stringBuilder = new StringBuilder();
                    foreach (var t in buffer)
                    {
                        stringBuilder.Append(t.ToString("x2"));
                    }

                    hashMd5 = stringBuilder.ToString();
                }

            } // 结束计算

            return hashMd5;
        }

        public static string GetMD5Hash(FileStream fileStream)
        {
            var md5 = MD5.Create();
            var buffer = md5.ComputeHash(fileStream);
            md5.Clear();
            // 将字节数组转换成十六进制的字符串形式
            var stringBuilder = new StringBuilder();
            foreach (var t in buffer)
            {
                stringBuilder.Append(t.ToString("x2"));
            }

            string hashMd5 = stringBuilder.ToString();
            return hashMd5;
        }
    }
}
