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

        /// <summary>
        /// 上传目录
        /// </summary>
        private readonly string _folder;
        private readonly ILogger<DownloadController> _logger;
        private IFileServerProvider _fileServerProvider;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"}
        };

        public DownloadController(IWebHostEnvironment hostingEnvironment, IFileServerProvider fileServerProvider,
            ILogger<DownloadController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileServerProvider = fileServerProvider;
            _folder = $@"{_hostingEnvironment.ContentRootPath}\UploadFolder";
            _logger = logger;
        }

        /// <summary>
        /// 文件流下载
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var path = $@"{_folder}\{fileName}";
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);

            // 响应头必须设置"Content Type"，否则浏览器会解析失败。
            return new FileStreamResult(memoryStream, _contentTypes[Path.GetExtension(path).ToLowerInvariant()]);
        }

        /// <summary>
        /// 文件流下载
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("{filePath}/{fileName}")]
        public async Task<IActionResult> Download(string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var fileProvider = _fileServerProvider.GetProvider($"/{filePath}");
            var fileInfo = fileProvider.GetFileInfo($"{fileName}");
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(fileInfo.PhysicalPath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);

            // 响应头必须设置"Content Type"，否则浏览器会解析失败。
            return new FileStreamResult(memoryStream, _contentTypes[Path.GetExtension(fileInfo.PhysicalPath).ToLowerInvariant()]);
        }
    }
}
