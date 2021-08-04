using System;
using System.Collections.Generic;

namespace FileServer.FileProvider
{
    public class FileProviderOptions
    {
        private readonly IList<FileProviderSchemeBuilder> _schemes = new List<FileProviderSchemeBuilder>();

        /// <summary>
        /// Returns the schemes in the order they were added (important for request handling priority)
        /// </summary>
        public IEnumerable<FileProviderSchemeBuilder> Schemes => _schemes;

        /// <summary>
        /// Maps schemes by name.
        /// </summary>
        public IDictionary<string, FileProviderSchemeBuilder> SchemeMap { get; } = new Dictionary<string, FileProviderSchemeBuilder>(StringComparer.Ordinal);

        /// <summary>
        /// Adds an <see cref="FileProviderScheme"/>.
        /// </summary>
        /// <param name="name">The name of the scheme being added.</param>
        /// <param name="configureBuilder">Configures the scheme.</param>
        public void AddScheme(string name, Action<FileProviderSchemeBuilder> configureBuilder)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (configureBuilder == null)
            {
                throw new ArgumentNullException(nameof(configureBuilder));
            }
            if (SchemeMap.ContainsKey(name))
            {
                throw new InvalidOperationException("Scheme already exists: " + name);
            }

            var builder = new FileProviderSchemeBuilder(name);
            configureBuilder(builder);
            _schemes.Add(builder);
            SchemeMap[name] = builder;
        }

        /// <summary>
        /// Adds an <see cref="FileProviderScheme"/>.
        /// </summary>
        /// <typeparam name="THandler">The <see cref="IBlobProvider"/> responsible for the scheme.</typeparam>
        /// <param name="name">The name of the scheme being added.</param>
        /// <param name="displayName">The display name for the scheme.</param>
        public void AddScheme<THandler>(string name, string displayName) where THandler : IBlobProvider
            => AddScheme(name, b =>
            {
                b.DisplayName = displayName;
                b.HandlerType = typeof(THandler);
            });

        /// <summary>
        /// Used as the fallback default scheme for all the other defaults.
        /// </summary>
        public string DefaultScheme { get; set; }


        /// <summary>
        /// Used as the default scheme by <see cref="IFileProviderService.FileProviderAsync(HttpContext, string)"/>.
        /// </summary>
        public string DefaultFileProviderScheme { get; set; }
    }
}
