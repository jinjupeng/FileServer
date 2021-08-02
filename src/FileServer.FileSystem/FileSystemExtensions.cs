using FileServer.FileProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FileServer.FileSystem
{
    public static class FileSystemExtensions
    {
        public static IServiceCollection AddFileSystem(this IServiceCollection services, IConfiguration cfg)
        {
            var option = new FileSystemBlobProviderOptions();
            var section = cfg.GetSection("FileServer:FileSystem");
            if (section == null)
            {
                throw new ArgumentNullException(nameof(FileSystemBlobProviderOptions));
            }

            section.Bind(option);

            services.AddSingleton(option);
            services.Configure<FileSystemBlobProviderOptions>(section);

            services.AddTransient<IBlobProvider, FileSystemBlobProvider>();
            services.AddTransient<IBlobFilePathCalculator, DefaultBlobFilePathCalculator>();

            return services;
        }
    }
}
