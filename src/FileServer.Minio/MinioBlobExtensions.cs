using FileServer.FileProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace FileServer.Minio
{
    public static class MinioBlobExtensions
    {
        public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration cfg)
        {
            var option = new MinioBlobOptions();
            var section = cfg.GetSection("FileServer:Minio");
            if (section == null)
            {
                throw new ArgumentNullException(nameof(MinioBlobOptions));
            }

            section.Bind(option);

            services.AddSingleton(option);
            services.Configure<MinioBlobOptions>(section);

            services.AddTransient<IFileProviderHandler, MinioBlobProvider>();
            services.AddTransient<IMinioBlobNameCalculator, DefaultMinioBlobNameCalculator>();

            return services;
        }
    }
}
