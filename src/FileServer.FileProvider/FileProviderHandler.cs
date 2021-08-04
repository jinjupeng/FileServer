using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public abstract class FileProviderHandler<TOptions> : IFileProviderHandler 
        where TOptions : FileProviderSchemeOptions, new()
    {
        public FileProviderScheme Scheme { get; private set; }
        public TOptions Options { get; private set; }
        protected ILogger Logger { get; }
        protected HttpContext Context { get; private set; }
        protected IOptionsMonitor<TOptions> OptionsMonitor { get; }

        public FileProviderHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger)
        {
            Logger = logger.CreateLogger(this.GetType().FullName);
            OptionsMonitor = options;
        }

        public async Task InitializeAsync(FileProviderScheme scheme, HttpContext context)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            Scheme = scheme;
            Context = context;

            Options = OptionsMonitor.Get(Scheme.Name);

            await InitializeHandlerAsync();
        }

        /// <summary>
        /// Called after options/events have been initialized for the handler to finish initializing itself.
        /// </summary>
        /// <returns>A task</returns>
        protected virtual Task InitializeHandlerAsync() => Task.CompletedTask;

        public virtual Task SaveAsync(BlobProviderSaveArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Stream> GetAsync(BlobProviderGetArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
