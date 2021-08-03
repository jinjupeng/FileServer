using System;

namespace FileServer.FileProvider
{
    public class FileProviderSchemeOptions
    {
        /// <summary>
        /// Check that the options are valid. Should throw an exception if things are not ok.
        /// </summary>
        public virtual void Validate() { }

        /// <summary>
        /// Checks that the options are valid for a specific scheme
        /// </summary>
        /// <param name="scheme">The scheme being validated.</param>
        public virtual void Validate(string scheme)
            => Validate();

        /// <summary>
        /// Instance used for events
        /// </summary>
        public object Events { get; set; }

        /// <summary>
        /// If set, will be used as the service type to get the Events instance instead of the property.
        /// </summary>
        public Type EventsType { get; set; }
    }
}
