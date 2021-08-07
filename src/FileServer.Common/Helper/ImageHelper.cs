using FileServer.Common.Enums;
using System;
using System.IO;

namespace FileServer.Common.Helper
{
    public static class ImageHelper
    {
        /// <summary>
        /// 判断是否是文件格式
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsImage(Stream stream)
        {
            try
            {
                BinaryReader reader = new BinaryReader(stream);
                string fileClass;
                byte buffer;
                buffer = reader.ReadByte();
                fileClass = buffer.ToString();
                buffer = reader.ReadByte();
                fileClass += buffer.ToString();
                reader.Close();
                var fileEx = Enum.GetValues(typeof(FileExtensionNameEnum));
                foreach (var fe in fileEx)
                {
                    if (Int32.Parse(fileClass) == (int)fe) return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
