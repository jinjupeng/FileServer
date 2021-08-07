using FileServer.Common.Helper;
using FileServer.FileProvider;
using FileServer.ImageResize.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileServer.ImageResize
{
    public class DynamicResizingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DynamicResizingMiddleware> _log;
        private readonly IFileProviderHandler _storage;
        private readonly IImageResizeService _resizer;

        public DynamicResizingMiddleware(RequestDelegate next, ILogger<DynamicResizingMiddleware> logger,
            IFileProviderHandler storage, IImageResizeService resizer)
        {
            _next = next;
            _log = logger;
            _storage = storage;
            _resizer = resizer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.QueryString.HasValue)
            {
                _log.LogDebug("No query string, no dynamic resizing");
                await _next(context);
                return;
            }
            var instructions = new ResizeInstructions(context.Request.QueryString);
            if (instructions.Width == null && instructions.Height == null)
            {
                _log.LogDebug("No resizing instruction found in query string, continue pipeline");
                await _next(context);
                return;
            }

            var blobName = WebUtility.UrlDecode(context.Request.Path.ToString().Substring(1)); // 移除第一位的"/"
            var blobProviderExistsArgs = new BlobProviderExistsArgs(blobName);
            var isFileExist = await _storage.ExistsAsync(blobProviderExistsArgs);
            if(!isFileExist)
            {
                _log.LogDebug("图片不存在");
                await _next(context);
                return;
            }
            var blobProviderGetArgs = new BlobProviderGetArgs(blobName);
            var fileStream = await _storage.GetOrNullAsync(blobProviderGetArgs);
            // 判断是否是图片格式
            if (!ImageHelper.IsImage(fileStream))
            {
                _log.LogDebug("不是图片格式");
                await _next(context);
                return;
            }

            try
            {
                _log.LogDebug("Do resizing");
                using (var tempFile = new MemoryStream())
                {
                    using (Stream original = fileStream)
                    {
                        _resizer.Resize(original, tempFile, instructions); 
                        tempFile.Position = 0;

                        var saveBlobName = $"{Path.GetDirectoryName(blobName)}/{Path.GetFileNameWithoutExtension(blobName)}_{instructions.Width}_{instructions.Height}{Path.GetExtension(blobName)}";

                        var responseUrl = $"{context.Request.Scheme}://{ context.Request.Host}/{saveBlobName}";
                        // 如果之前不存在相同的裁切格式图片，则新增一个，否则直接返回响应
                        if (!await _storage.ExistsAsync(new BlobProviderExistsArgs(saveBlobName)))
                        {
                            var saveArgs = new BlobProviderSaveArgs(saveBlobName, tempFile, true);
                            await _storage.SaveAsync(saveArgs);
                        }

                        context.Response.Redirect(responseUrl, true);
                        return;
                    }
                }
            }
            catch (Exception ex) // 图片裁切异常
            {
                _log.LogError("Exception during dynamic resizing, redirect to the origin {0}", ex);
                context.Response.Redirect($"{context.Request.Scheme}://{ context.Request.Host}/{blobName}", false);
            }

            await _next(context);
        }
    }
}
