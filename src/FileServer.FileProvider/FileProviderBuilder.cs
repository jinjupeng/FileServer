using Microsoft.Extensions.DependencyInjection;
using System;

namespace FileServer.FileProvider
{
    public class FileProviderBuilder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="services">The services being configured.</param>
        public FileProviderBuilder(IServiceCollection services)
            => Services = services;

        /// <summary>
        /// The services being configured.
        /// </summary>
        public virtual IServiceCollection Services { get; }


        private FileProviderBuilder AddFileProviderHelper<TOptions, THandler>(string fileServerScheme, string displayName, Action<TOptions> configureOptions)
            where TOptions : FileProviderSchemeOptions, new()
            where THandler : class, IFileProviderHandler
        {
            Services.Configure<FileProviderOptions>(o =>
            {
                o.AddScheme(fileServerScheme, scheme => {
                    scheme.HandlerType = typeof(THandler);
                    scheme.DisplayName = displayName;
                });
            });
            if (configureOptions != null)
            {
                Services.Configure(fileServerScheme, configureOptions);
            }
            Services.AddOptions<TOptions>(fileServerScheme).Validate(o => {
                o.Validate(fileServerScheme);
                return true;
            });
            Services.AddTransient<THandler>();
            return this;
        }

        public virtual FileProviderBuilder AddScheme<TOptions, THandler>(string fileServerScheme, string displayName, Action<TOptions> configureOptions)
            where TOptions : FileProviderSchemeOptions, new()
            where THandler : FileProviderHandler<TOptions>
            => AddFileProviderHelper<TOptions, THandler>(fileServerScheme, displayName, configureOptions);
    }
}
