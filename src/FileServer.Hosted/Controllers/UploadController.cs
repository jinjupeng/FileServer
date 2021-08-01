using FileServer.Common.Extensions;
using FileServer.FileProvider;
using FileServer.FileSystem;
using FileServer.Hosted.Filters;
using FileServer.OSS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.Hosted.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        /*
         * 1、切片上传
         * 2、多线程上传
         * 3、流式上传
         * https://docs.microsoft.com/zh-cn/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1
         * https://blog.johnwu.cc/article/ironman-day23-asp-net-core-upload-download-files.html
         * https://stackoverflow.com/questions/62502286/uploading-and-downloading-large-files-in-asp-net-core-3-1
         * https://www.jb51.net/article/174873.htm
         * https://www.cnblogs.com/liuxiaoji/p/10266609.html
         * https://gitee.com/loogn/UploadServer?_from=gitee_search
         * https://www.cnblogs.com/Hangle/p/10233872.html
         * https://www.cnblogs.com/liyouming/p/13341173.html
         * [虚拟目录](https://www.cnblogs.com/EminemJK/p/13362368.html)
         * 4、和OSS结合在一起使用
         */

        /// <summary>
        /// 上传目录
        /// </summary>
        private readonly string _targetFilePath;
        private IFileServerProvider _fileServerProvider;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<UploadController> _logger;
        private readonly IOSSProvider _provider;
        private readonly OSSOptions _options;
        private readonly IBlobProvider _fileSystemBlobProvider;

        public UploadController(IWebHostEnvironment hostingEnvironment, IFileServerProvider fileServerProvider,
            ILogger<UploadController> logger, IOSSProvider provider,
            IOptions<OSSOptions> options, IBlobProvider fileSystemBlobProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileServerProvider = fileServerProvider;
            _logger = logger;
            _provider = provider;
            _options = options.Value;
            _fileSystemBlobProvider = fileSystemBlobProvider;

            // 把上传目录设为：wwwroot\UploadFolder
            _targetFilePath = $@"{_hostingEnvironment.ContentRootPath}\UploadFolder";
            if (!Directory.Exists(_targetFilePath))
            {
                Directory.CreateDirectory(_targetFilePath);
            }
        }

        /// <summary>
        /// 流式文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadingStream")]
        [DisableFormValueModelBindingFilter]
        public async Task<IActionResult> UploadingStream()
        {
            //获取boundary
            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
            //得到reader
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            //{ BodyLengthLimit = 2000 };//
            var section = await reader.ReadNextSectionAsync();

            //读取section
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                if (hasContentDispositionHeader)
                {
                    // 生成随机文件名
                    var trustedFileNameForFileStorage = Path.GetRandomFileName();
                    await section.Body.WriteFileAsync(Path.Combine(_targetFilePath, trustedFileNameForFileStorage));
                }
                section = await reader.ReadNextSectionAsync();
            }
            return Created(nameof(UploadController), null);
        }

        /// <summary>
        /// 缓存式文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("UploadingFormFile")]
        public async Task<IActionResult> UploadingFormFile(IFormFile file)
        {
            var fileProvider = _fileServerProvider.GetProvider($"{_options.RequestPath}");
            using (var stream = file.OpenReadStream())
            {
                await stream.WriteFileAsync(Path.Combine(_options.UploadPath, file.FileName));
            }
            var fileInfo = fileProvider.GetFileInfo($"{file.FileName}");
            _options.FileInfo = new FileInfo(fileInfo.PhysicalPath);
            return Ok(await Task.FromResult(_provider.GetFileUrl(_options)));
        }

        /// <summary>
        /// 缓存式文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("UploadingFormFile2")]
        public async Task<IActionResult> UploadingFormFile2(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var blobProviderSaveArgs = new BlobProviderSaveArgs("files/123.pdf", stream);
            await _fileSystemBlobProvider.SaveAsync(blobProviderSaveArgs);
            return Ok();
        }
    }
}
