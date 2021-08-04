using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace FileServer.FileProvider
{
    public static class FileProviderExtensions
    {
        public static FileProviderBuilder AddFileProvider(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddScoped<IFileProviderService, FileProviderService>();
            services.TryAddSingleton<IFileProviderSchemeProvider, FileProviderSchemeProvider>();
            services.TryAddScoped<IFileProviderHandlerProvider, FileProviderHandlerProvider>();

            return new FileProviderBuilder(services);
        }

        public static FileProviderBuilder AddFileProvider(this IServiceCollection services, string defaultScheme)
            => services.AddFileProvider(o => o.DefaultScheme = defaultScheme);

        public static FileProviderBuilder AddFileProvider(this IServiceCollection services, Action<FileProviderOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var builder = services.AddFileProvider();
            services.Configure(configureOptions);
            return builder;
        }
    }
}
