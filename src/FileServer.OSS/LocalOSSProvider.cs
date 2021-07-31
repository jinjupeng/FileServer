using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.OSS
{
    public class LocalOSSProvider : IOSSProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LocalOSSProvider> _logger;

        public LocalOSSProvider(IHttpContextAccessor httpContextAccessor, ILogger<LocalOSSProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public ValueTask<bool> Delete(OSSOptions options)
        {
            var result = false;
            if (File.Exists(options.FullPath))
            {
                File.Delete(options.FullPath);
                result = true;
            }

            return new ValueTask<bool>(result);
        }

        public string GetFileUrl(OSSOptions options)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            options.FileUrl = $"{request.Scheme}://{request.Host}{options.RequestPath}/{options.FileInfo.Name}";
            return options.FileUrl;
        }
    }
}
