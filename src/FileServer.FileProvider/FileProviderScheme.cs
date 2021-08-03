using System;

namespace FileServer.FileProvider
{
    public class FileProviderScheme
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name for the authentication scheme.</param>
        /// <param name="displayName">The display name for the fileProvider scheme.</param>
        /// <param name="handlerType">The <see cref="IBlobProvider"/> type that handles this scheme.</param>
        public FileProviderScheme(string name, string displayName, Type handlerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }
            if (!typeof(IBlobProvider).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException("handlerType must implement IBlobProvider.");
            }

            Name = name;
            HandlerType = handlerType;
            DisplayName = displayName;
        }

        /// <summary>
        /// The name of the authentication scheme.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The display name for the scheme. Null is valid and used for non user facing schemes.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The <see cref="IBlobProvider"/> type that handles this scheme.
        /// </summary>
        public Type HandlerType { get; }
    }
}
