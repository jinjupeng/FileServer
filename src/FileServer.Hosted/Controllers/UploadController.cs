using FileServer.FileProvider;
using FileServer.Hosted.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace FileServer.Hosted.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        /*
         * 参考链接：
         * https://docs.microsoft.com/zh-cn/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1
         * https://blog.johnwu.cc/article/ironman-day23-asp-net-core-upload-download-files.html
         * https://stackoverflow.com/questions/62502286/uploading-and-downloading-large-files-in-asp-net-core-3-1
         * https://www.jb51.net/article/174873.htm
         * https://www.cnblogs.com/liuxiaoji/p/10266609.html
         * https://gitee.com/loogn/UploadServer?_from=gitee_search
         * https://www.cnblogs.com/Hangle/p/10233872.html
         * https://www.cnblogs.com/liyouming/p/13341173.html
         * https://www.cnblogs.com/EminemJK/p/13362368.html
         */

        /*
         * 1、切片上传
         * 2、多线程上传
         * 3、流式上传
         * 4、和OSS结合在一起使用
         * 5、断点续传
         */

        private readonly ILogger<UploadController> _logger;
        private readonly IFileProviderHandler _fileSystemBlobProvider;

        public UploadController(IWebHostEnvironment hostingEnvironment, ILogger<UploadController> logger,
            IFileProviderHandler fileSystemBlobProvider)
        {
            _logger = logger;
            _fileSystemBlobProvider = fileSystemBlobProvider;
        }

        /// <summary>
        /// 流式文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost("Stream")]
        [DisableFormValueModelBindingFilter]
        public async Task<IActionResult> Stream()
        {
            //获取boundary
            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
            //得到reader
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            //{ BodyLengthLimit = 2000 };//
            var section = await reader.ReadNextSectionAsync();
            var filePath = string.Empty;

            //读取section
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                if (hasContentDispositionHeader)
                {
                    var fileSection = section.AsFileSection();
                    var fileName = fileSection.FileName;
                    filePath = $"files/{fileName}";
                    var blobProviderSaveArgs = new BlobProviderSaveArgs(filePath, section.Body, overrideExisting : true);
                    await _fileSystemBlobProvider.SaveAsync(blobProviderSaveArgs);
                    //await section.Body.WriteFileAsync(Path.Combine(_targetFilePath, "{fileName}"));
                }
                section = await reader.ReadNextSectionAsync();
            }
            return Ok(filePath);
        }

        /// <summary>
        /// 缓存式文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("FormFile")]
        public async Task<IActionResult> FormFile(IFormFile file)
        {
            var filePath = $"files/{file.FileName}";
            var stream = file.OpenReadStream();
            var blobProviderSaveArgs = new BlobProviderSaveArgs(filePath, stream, overrideExisting: true);
            await _fileSystemBlobProvider.SaveAsync(blobProviderSaveArgs);
            return Ok(filePath);
        }
    }
}
