using FileServer.FileProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace FileServer.FileSystem
{
    public static class FileSystemServiceCollectionExtensions
    {
        //public static IServiceCollection AddFileSystem(this IServiceCollection services, IConfiguration cfg)
        //{
        //    var option = new FileSystemBlobOptions();
        //    var section = cfg.GetSection("FileServer:FileSystem");
        //    if (section == null)
        //    {
        //        throw new ArgumentNullException(nameof(FileSystemBlobOptions));
        //    }

        //    section.Bind(option);

        //    services.AddSingleton(option);
        //    services.Configure<FileSystemBlobOptions>(section);

        //    services.AddTransient<IBlobProvider, FileSystemBlobProvider>();
        //    services.AddTransient<IBlobFilePathCalculator, DefaultBlobFilePathCalculator>();

        //    return services;
        //}


        public static IServiceCollection AddFileSystem(this IServiceCollection services, Action<FileSystemOptions> configureOptions)
        {
            services.TryAddTransient<IFilePathCalculator, DefaultFilePathCalculator>();
            services.TryAddTransient<IFileProviderHandler, FileSystemProvider>();

            if (configureOptions != null)
            {
                services.ConfigureFileSystem(configureOptions);
            }
            services.AddOptions<FileSystemOptions>().Validate(o => {
                o.Validate();
                return true;
            });
            return services;
        }

        public static void ConfigureFileSystem(this IServiceCollection services, Action<FileSystemOptions> setupAction)
        {
            services.Configure(setupAction);
        }
    }
}
