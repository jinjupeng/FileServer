using System.ComponentModel;

namespace FileServer.OSS
{
    public enum ProviderType
    {
        [Description("本地存储")]
        Local,

        /// <summary>
        /// 阿里云
        /// </summary>
        [Description("阿里云OSS")]
        Aliyun,

    }
}
