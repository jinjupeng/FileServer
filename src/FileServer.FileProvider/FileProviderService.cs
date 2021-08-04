using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Implements <see cref="IFileProviderService"/>.
    /// </summary>
    public class FileProviderService : IFileProviderService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="IFileProviderSchemeProvider"/>.</param>
        /// <param name="handlers">The <see cref="IFileProviderHandlerProvider"/>.</param>
        /// <param name="options">The <see cref="FileProviderOptions"/>.</param>
        public FileProviderService(IFileProviderSchemeProvider schemes, IFileProviderHandlerProvider handlers, IOptions<FileProviderOptions> options)
        {
            Schemes = schemes;
            Handlers = handlers;
            Options = options.Value;
        }

        /// <summary>
        /// Used to lookup FileProviderSchemes.
        /// </summary>
        public IFileProviderSchemeProvider Schemes { get; }

        /// <summary>
        /// Used to resolve IFileProviderHandler instances.
        /// </summary>
        public IFileProviderHandlerProvider Handlers { get; }


        /// <summary>
        /// The <see cref="FileProviderOptions"/>.
        /// </summary>
        public FileProviderOptions Options { get; }

        public async Task<bool> DeleteAsync(HttpContext context, string scheme, BlobProviderDeleteArgs args)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultFileProviderSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No fileProviderScheme was specified, and there was no DefaultFileProviderScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                //throw await CreateMissingHandlerException(scheme);
            }

            var result = await handler.DeleteAsync(args);
            return result;
        }

        public async Task<bool> ExistsAsync(HttpContext context, string scheme, BlobProviderExistsArgs args)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultFileProviderSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No fileProviderScheme was specified, and there was no DefaultFileProviderScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                //throw await CreateMissingHandlerException(scheme);
            }

            var result = await handler.ExistsAsync(args);
            return result;
        }

        public async Task<Stream> GetAsync(HttpContext context, string scheme, BlobProviderGetArgs args)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultFileProviderSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No fileProviderScheme was specified, and there was no DefaultFileProviderScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                //throw await CreateMissingHandlerException(scheme);
            }

            var result = await handler.GetAsync(args);
            return result;
        }

        public async Task<Stream> GetOrNullAsync(HttpContext context, string scheme, BlobProviderGetArgs args)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultFileProviderSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No fileProviderScheme was specified, and there was no DefaultFileProviderScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                //throw await CreateMissingHandlerException(scheme);
            }

            var result = await handler.GetOrNullAsync(args);
            return result;
        }

        public async Task SaveAsync(HttpContext context, string scheme, BlobProviderSaveArgs args)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultFileProviderSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No fileProviderScheme was specified, and there was no DefaultFileProviderScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                //throw await CreateMissingHandlerException(scheme);
            }

            await handler.SaveAsync(args);
        }
    }
}
