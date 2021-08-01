using FileServer.FileProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.Hosted.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        /*
         * 1、多线程下载
         * 2、断电续传
         * 3、限速下载
         * 4、多文件下载
         * 5、下载出错自动重试
         * 6、暂停、继续和取消
         * 7、url重定义下载
         * 8、网络异常问题处理
         */

        private readonly ILogger<DownloadController> _logger;
        private IFileServerProvider _fileServerProvider;
        private readonly IBlobProvider _fileSystemBlobProvider;
        private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".pdf", "application/pdf"}
        };

        public DownloadController(IFileServerProvider fileServerProvider, ILogger<DownloadController> logger, 
            IBlobProvider fileSystemBlobProvider)
        {
            _fileServerProvider = fileServerProvider;
            _logger = logger;
            _fileSystemBlobProvider = fileSystemBlobProvider;
        }

        /// <summary>
        /// 文件流下载
        /// </summary>
        /// <param name="virtualFilePath">文件虚拟路径</param>
        /// <returns></returns>
        [HttpGet("stream")]
        public async Task<IActionResult> Download([FromQuery] string virtualFilePath)
        {
            if (string.IsNullOrEmpty(virtualFilePath))
            {
                return NotFound();
            }
            var getFileArgs = new BlobProviderGetArgs(virtualFilePath);
            var fileStream = await _fileSystemBlobProvider.GetOrNullAsync(getFileArgs);
            // 获取文件名
            var fileName = Path.GetFileName(virtualFilePath);

            // 获取文件扩展名
            var fileExtension = Path.GetExtension(virtualFilePath).ToLowerInvariant();
            // 默认响应类型
            var contextType = "application/octet-stream";
            if (_contentTypes.ContainsKey(fileExtension))
            {
                contextType = _contentTypes[fileExtension];
            }

            // 响应头必须设置"Content Type"，否则浏览器会解析失败。
            return File(fileStream, contextType, fileName);
        }

        ///// <summary>
        ///// 文件流下载
        ///// </summary>
        ///// <param name="filePath"></param>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> Download([FromQuery] string filePath, string fileName)
        //{
        //    if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
        //    {
        //        return NotFound();
        //    }

        //    var fileProvider = _fileServerProvider.GetProvider($"/{filePath}");
        //    var fileInfo = fileProvider.GetFileInfo($"{fileName}");
        //    var memoryStream = new MemoryStream();
        //    using (var stream = new FileStream(fileInfo.PhysicalPath, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memoryStream);
        //    }
        //    memoryStream.Seek(0, SeekOrigin.Begin);

        //    // 响应头必须设置"Content Type"，否则浏览器会解析失败。
        //    return new FileStreamResult(memoryStream, _contentTypes[Path.GetExtension(fileInfo.PhysicalPath).ToLowerInvariant()]);
        //}
    }
}
