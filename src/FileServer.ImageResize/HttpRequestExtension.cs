using System;
using Microsoft.AspNetCore.Http;

namespace FileServer.ImageResize
{
    internal static class HttpRequestExtension
    {
        public static Uri GetAbsoluteUri(this HttpRequest request)
        {
            return new Uri(string.Concat(
                request.Scheme,
                "://",
                request.Host.ToUriComponent(),
                request.PathBase.ToUriComponent(),
                request.Path.ToUriComponent(),
                request.QueryString.ToUriComponent()));
        }
    }
}
