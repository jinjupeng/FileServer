using FileServer.FileProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FileServer.FileSystem
{
    public static class FileSystemBlobExtensions
    {
        public static IServiceCollection AddFileSystem(this IServiceCollection services, IConfiguration cfg)
        {
            var option = new FileSystemBlobOptions();
            var section = cfg.GetSection("FileServer:FileSystem");
            if (section == null)
            {
                throw new ArgumentNullException(nameof(FileSystemBlobOptions));
            }

            section.Bind(option);

            services.AddSingleton(option);
            services.Configure<FileSystemBlobOptions>(section);

            services.AddTransient<IBlobProvider, FileSystemBlobProvider>();
            services.AddTransient<IBlobFilePathCalculator, DefaultBlobFilePathCalculator>();

            return services;
        }
    }
}
