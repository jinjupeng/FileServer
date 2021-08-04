using FileServer.FileProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace FileServer.FileSystem
{
    public static class FileSystemBlobExtensions
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

        public static FileProviderBuilder AddFileSystem(this FileProviderBuilder builder)
            => builder.AddFileSystem(_ => { });

        public static FileProviderBuilder AddFileSystem(this FileProviderBuilder builder, Action<FileSystemBlobOptions> configureOptions)
            => builder.AddFileSystem(FileSystemBlobDefaults.FileProviderScheme, configureOptions);

        public static FileProviderBuilder AddFileSystem(this FileProviderBuilder builder, string fileProviderScheme, Action<FileSystemBlobOptions> configureOptions)
            => builder.AddFileSystem(fileProviderScheme, displayName: null, configureOptions);

        public static FileProviderBuilder AddFileSystem(this FileProviderBuilder builder, string fileProviderScheme, string displayName, Action<FileSystemBlobOptions> configureOptions)
        {
            builder.Services.TryAddTransient<IBlobFilePathCalculator, DefaultBlobFilePathCalculator>();
            builder.Services.TryAddTransient<IFileProviderHandler, FileSystemBlobProvider>();
            return builder.AddScheme<FileSystemBlobOptions, FileSystemBlobProvider>(fileProviderScheme, displayName, configureOptions);
        }
    }
}
