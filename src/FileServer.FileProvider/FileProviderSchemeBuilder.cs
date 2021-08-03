using System;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Used to build <see cref="FileProviderScheme"/>s.
    /// </summary>
    public class FileProviderSchemeBuilder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the scheme being built.</param>
        public FileProviderSchemeBuilder(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the scheme being built.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The display name for the scheme being built.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The <see cref="IBlobProvider"/> type responsible for this scheme.
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// Builds the <see cref="FileProviderScheme"/> instance.
        /// </summary>
        /// <returns></returns>
        public FileProviderScheme Build() => new FileProviderScheme(Name, DisplayName, HandlerType);
    }
}
