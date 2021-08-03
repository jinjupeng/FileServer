using FileServer.FileProvider;
using System;

namespace FileServer.Minio
{
    public class MinioBlobOptions : FileProviderSchemeOptions
    {
        public string BucketName { get; set; }

        /// <summary>
        /// endPoint is an URL, domain name, IPv4 address or IPv6 address.
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// accessKey is like user-id that uniquely identifies your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// secretKey is the password to your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// connect to  to MinIO Client object to use https instead of http
        /// </summary>
        public bool WithSSL { get; set; } = false;

        /// <summary>
        /// Default value: false.
        /// </summary>
        public bool CreateBucketIfNotExists { get; set; } = false;


        public override void Validate()
        {
            if (string.IsNullOrEmpty(BucketName))
            {
                throw new ArgumentException($"The {nameof(BucketName)} option must be provided.");
            }

            if (string.IsNullOrEmpty(EndPoint))
            {
                throw new ArgumentException($"The {nameof(EndPoint)} option must be provided.");
            }

            if (string.IsNullOrEmpty(AccessKey))
            {
                throw new ArgumentException($"The {nameof(AccessKey)} option must be provided.");
            }

            if (string.IsNullOrEmpty(SecretKey))
            {
                throw new ArgumentException($"The {nameof(SecretKey)} option must be provided.");
            }
        }
    }
}
