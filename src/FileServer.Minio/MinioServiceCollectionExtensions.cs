using FileServer.FileProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace FileServer.Minio
{
    public static class MinioServiceCollectionExtensions
    {
        //public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration cfg)
        //{
        //    var option = new MinioBlobOptions();
        //    var section = cfg.GetSection("FileServer:Minio");
        //    if (section == null)
        //    {
        //        throw new ArgumentNullException(nameof(MinioBlobOptions));
        //    }

        //    section.Bind(option);

        //    services.AddSingleton(option);
        //    services.Configure<MinioBlobOptions>(section);

        //    services.AddTransient<IFileProviderHandler, MinioBlobProvider>();
        //    services.AddTransient<IMinioBlobNameCalculator, DefaultMinioBlobNameCalculator>();

        //    return services;
        //}

        public static IServiceCollection AddMinio(this IServiceCollection services, Action<MinioOptions> configureOptions)
        {
            services.TryAddTransient<IFileProviderHandler, MinioProvider>();
            services.TryAddTransient<IFilePathCalculator, DefaultMinioNameCalculator>();

            if (configureOptions != null)
            {
                services.ConfigureMinio(configureOptions);
            }
            services.AddOptions<MinioOptions>().Validate(o => {
                o.Validate();
                return true;
            });
            return services;
        }

        public static void ConfigureMinio(this IServiceCollection services, Action<MinioOptions> setupAction)
        {
            services.Configure(setupAction);
        }
    }
}
