using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Implements <see cref="IFileProviderSchemeProvider"/>.
    /// </summary>
    public class FileProviderSchemeProvider : IFileProviderSchemeProvider
    {
        /// <summary>
        /// Creates an instance of <see cref="FileProviderSchemeProvider"/>
        /// using the specified <paramref name="options"/>,
        /// </summary>
        /// <param name="options">The <see cref="FileProviderOptions"/> options.</param>
        public FileProviderSchemeProvider(IOptions<FileProviderOptions> options)
            : this(options, new Dictionary<string, FileProviderScheme>(StringComparer.Ordinal))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="FileProviderSchemeProvider"/>
        /// using the specified <paramref name="options"/> and <paramref name="schemes"/>.
        /// </summary>
        /// <param name="options">The <see cref="FileProviderOptions"/> options.</param>
        /// <param name="schemes">The dictionary used to store authentication schemes.</param>
        protected FileProviderSchemeProvider(IOptions<FileProviderOptions> options, IDictionary<string, FileProviderScheme> schemes)
        {
            _options = options.Value;

            _schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _requestHandlers = new List<FileProviderScheme>();

            foreach (var builder in _options.Schemes)
            {
                var scheme = builder.Build();
                AddScheme(scheme);
            }
        }

        private readonly FileProviderOptions _options;
        private readonly object _lock = new object();

        private readonly IDictionary<string, FileProviderScheme> _schemes;
        private readonly List<FileProviderScheme> _requestHandlers;
        // Used as a safe return value for enumeration apis
        private IEnumerable<FileProviderScheme> _schemesCopy = Array.Empty<FileProviderScheme>();
        private IEnumerable<FileProviderScheme> _requestHandlersCopy = Array.Empty<FileProviderScheme>();


        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IFileProviderService.FileProviderAsync(HttpContext, string)"/>.
        /// This is typically specified via <see cref="FileProviderOptions.DefaultFileProviderScheme"/>.
        /// Otherwise, this will fallback to <see cref="FileProviderOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IFileProviderService.FileProviderAsync(HttpContext, string)"/>.</returns>
        public virtual Task<FileProviderScheme> GetDefaultFileProviderSchemeAsync()
            => _options.DefaultFileProviderScheme != null
            ? GetSchemeAsync(_options.DefaultFileProviderScheme)
            : GetDefaultSchemeAsync();

        private Task<FileProviderScheme> GetDefaultSchemeAsync()
            => _options.DefaultScheme != null
            ? GetSchemeAsync(_options.DefaultScheme)
            : Task.FromResult<FileProviderScheme>(null);


        /// <summary>
        /// Returns the <see cref="FileProviderScheme"/> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme.</param>
        /// <returns>The scheme or null if not found.</returns>
        public virtual Task<FileProviderScheme> GetSchemeAsync(string name)
            => Task.FromResult(_schemes.ContainsKey(name) ? _schemes[name] : null);

        /// <summary>
        /// Returns the schemes in priority order for request handling.
        /// </summary>
        /// <returns>The schemes in priority order for request handling</returns>
        public virtual Task<IEnumerable<FileProviderScheme>> GetRequestHandlerSchemesAsync()
            => Task.FromResult(_requestHandlersCopy);

        /// <summary>
        /// Registers a scheme for use by <see cref="IFileProviderService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public virtual void AddScheme(FileProviderScheme scheme)
        {
            if (_schemes.ContainsKey(scheme.Name))
            {
                throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(scheme.Name))
                {
                    throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
                }
                if (typeof(IFileProviderRequestHandler).IsAssignableFrom(scheme.HandlerType))
                {
                    _requestHandlers.Add(scheme);
                    _requestHandlersCopy = _requestHandlers.ToArray();
                }
                _schemes[scheme.Name] = scheme;
                _schemesCopy = _schemes.Values.ToArray();
            }
        }

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="IFileProviderService"/>.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme being removed.</param>
        public virtual void RemoveScheme(string name)
        {
            if (!_schemes.ContainsKey(name))
            {
                return;
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(name))
                {
                    var scheme = _schemes[name];
                    if (_requestHandlers.Remove(scheme))
                    {
                        _requestHandlersCopy = _requestHandlers.ToArray();
                    }
                    _schemes.Remove(name);
                    _schemesCopy = _schemes.Values.ToArray();
                }
            }
        }

        public virtual Task<IEnumerable<FileProviderScheme>> GetAllSchemesAsync()
            => Task.FromResult(_schemesCopy);
    }
}
