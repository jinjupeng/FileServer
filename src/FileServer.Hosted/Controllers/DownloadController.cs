using FileServer.Common.Helper;
using FileServer.FileProvider;
using FileServer.VirtualFileSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
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
         * 2、断点续传
         * 3、限速下载
         * 4、多文件下载
         * 5、下载出错自动重试
         * 6、暂停、继续和取消
         * 7、url重定义下载
         * 8、网络异常问题处理
         */

        private readonly ILogger<DownloadController> _logger;
        private IFileServerProvider _fileServerProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileProviderHandler _fileSystemBlobProvider;
        private readonly IFilePathCalculator _filePathCalculator;
        private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".pdf", "application/pdf"}
        };

        public DownloadController(IFileServerProvider fileServerProvider, ILogger<DownloadController> logger,
            IFileProviderHandler fileSystemBlobProvider, IHttpContextAccessor contextAccessor,
            IFilePathCalculator filePathCalculator)
        {
            _fileServerProvider = fileServerProvider;
            _logger = logger;
            _fileSystemBlobProvider = fileSystemBlobProvider;
            _contextAccessor = contextAccessor;
            _filePathCalculator = filePathCalculator;
        }

        /// <summary>
        /// 文件流下载，不支持断点续传
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

            #region 文件流下载，不支持断点续传

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

            #endregion

            #region 切片下载，不支持断点续传

            ////100K 每次读取文件，只读取100K，这样可以缓解服务器的压力 
            //const int ChunkSize = 102400;
            ////设置ContentType为二进制流
            //Response.ContentType = "application/octet-stream";
            //Response.Headers[HeaderNames.AcceptRanges] = "bytes";
            ////返回的状态为200
            //Response.StatusCode = StatusCodes.Status200OK;
            ////设置文件名（注意需要编码以防止一些浏览器中的中文乱码）
            //var contentDisposition = new ContentDispositionHeaderValue("attachment");
            //contentDisposition.SetHttpFileName(fileName);
            //Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            //using (Response.Body)
            //{
            //    //读取文件的大小
            //    long contentLength = fileStream.Length;
            //    Response.Headers[HeaderNames.ContentLength] = contentLength.ToString();
            //    //已发送的文件大小
            //    long hasRead = 0;
            //    //若已发送的文件大小，小于文件的大小，继续发送
            //    while (hasRead < contentLength)
            //    {
            //        //若下载已被取消，跳出循环
            //        if (HttpContext.RequestAborted.IsCancellationRequested)
            //        {
            //            break;
            //        }
            //        byte[] buffer = new byte[ChunkSize];
            //        //从文件流中读取bufferSize大小，到缓冲区中
            //        int currentRead = fileStream.Read(buffer, 0, ChunkSize);
            //        //将缓冲区输出
            //        await Response.Body.WriteAsync(buffer, 0, currentRead);
            //        //输出完成后，释放服务器内存空间
            //        await Response.Body.FlushAsync();
            //        //累加已经读取的大小
            //        hasRead += currentRead;
            //    }
            //}

            //return new EmptyResult();

            #endregion
        }

        /// <summary>
        /// 分片下载，支持断点续传
        /// </summary>
        /// <param name="virtualFilePath">文件虚拟路径</param>
        /// <returns></returns>
        [HttpGet("chunk")]
        public async Task<IActionResult> ChunkDownload([FromQuery] string virtualFilePath)
        {
            if (string.IsNullOrEmpty(virtualFilePath))
            {
                return NotFound();
            }
            //var filePath = @"D:\\Share\\Fonts.zip";
            var getFileArgs = new BlobProviderGetArgs(virtualFilePath);
            var filePath = _filePathCalculator.Calculate(getFileArgs);
            await DownloadHelper.DownloadFileAsync(_contextAccessor.HttpContext, filePath, 1024000);
            return new EmptyResult();
        }
    }
}
