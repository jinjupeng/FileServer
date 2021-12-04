using FileServer.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.Hosted
{
    public static class UploadHelper
    {
        public static async Task<string> StreamUpload(HttpRequest request)
        {
            var filePath = string.Empty;
            // 获取boundary
            var boundary = HeaderUtilities.RemoveQuotes(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse(request.ContentType).Boundary).Value;
            // var boundary = request.GetMultipartBoundary();
            if (string.IsNullOrWhiteSpace(boundary))
            {
                // 说明不是一个文件流传输
                return filePath;
            }

            // 得到reader
            var reader = new MultipartReader(boundary, request.Body);
            // 获取部分内容
            var section = await reader.ReadNextSectionAsync();

            // 读取section
            while (section != null)
            {
                await using var memoryStream = new MemoryStream();
                await section.Body.CopyToAsync(memoryStream);

                if(memoryStream.Length == 0)
                {
                    return filePath;
                }
                if(memoryStream.Length > 1024 * 1024 * 3)
                {
                    return filePath;
                }

                var hasContentDispositionHeader = Microsoft.Net.Http.Headers.ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                if (hasContentDispositionHeader)
                {
                    var fileSection = section.AsFileSection();
                    var fileName = fileSection.FileName;
                    var fileDir = "D:/UploadByStream";
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    filePath = Path.Combine(fileDir, fileName);
                    await section.Body.WriteFileAsync(filePath);
                }
                section = await reader.ReadNextSectionAsync();
            }

            return filePath;
        }
    }
}
