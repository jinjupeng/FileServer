using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    public class FileProviderHandlerProvider : IFileProviderHandlerProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="IFileProviderSchemeProvider"/>.</param>
        public FileProviderHandlerProvider(IFileProviderSchemeProvider schemes)
        {
            Schemes = schemes;
        }

        /// <summary>
        /// The <see cref="IFileProviderHandlerProvider"/>.
        /// </summary>
        public IFileProviderSchemeProvider Schemes { get; }

        // handler instance cache, need to initialize once per request
        private Dictionary<string, IFileProviderHandler> _handlerMap = new Dictionary<string, IFileProviderHandler>(StringComparer.Ordinal);

        /// <summary>
        /// Returns the handler instance that will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="filrProviderScheme">The name of the filrProvider scheme being handled.</param>
        /// <returns>The handler instance.</returns>
        public async Task<IFileProviderHandler> GetHandlerAsync(HttpContext context, string filrProviderScheme)
        {
            if (_handlerMap.ContainsKey(filrProviderScheme))
            {
                return _handlerMap[filrProviderScheme];
            }

            var scheme = await Schemes.GetSchemeAsync(filrProviderScheme);
            if (scheme == null)
            {
                return null;
            }
            var handler = (context.RequestServices.GetService(scheme.HandlerType) ??
                ActivatorUtilities.CreateInstance(context.RequestServices, scheme.HandlerType))
                as IFileProviderHandler;
            if (handler != null)
            {
                await handler.InitializeAsync(scheme, context);
                _handlerMap[filrProviderScheme] = handler;
            }
            return handler;
        }
    }
}
