using System;
using System.Threading.Tasks;

namespace FileServer.OSS
{
    public interface IOSSProvider
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        ValueTask<bool> Delete(OSSOptions options);

        /// <summary>
        /// 获取文件访问url
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        string GetFileUrl(OSSOptions options);
    }
}
