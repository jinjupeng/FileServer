using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FileServer.ImageResize
{
    public static class ImageResizeServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicResizing(this IServiceCollection services, Action<ResizingOptions> configureOptions)
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            services.AddSingleton<IImageResizeService, ImageResizeService>();
            return services;
        }

        public static IApplicationBuilder UseDynamicResizing(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DynamicResizingMiddleware>();
        }
    }
}
