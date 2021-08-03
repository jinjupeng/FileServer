using System;
using System.Collections.Generic;
using System.Text;

namespace FileServer.Minio
{
    public class MinioBlobOptions
    {
        public string BucketName
        {
            get;set;
        }

        /// <summary>
        /// endPoint is an URL, domain name, IPv4 address or IPv6 address.
        /// </summary>
        public string EndPoint
        {
            get; set;
        }

        /// <summary>
        /// accessKey is like user-id that uniquely identifies your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string AccessKey
        {
            get; set;
        }

        /// <summary>
        /// secretKey is the password to your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string SecretKey
        {
            get; set;
        }

        /// <summary>
        ///connect to  to MinIO Client object to use https instead of http
        /// </summary>
        public bool WithSSL
        {
            get; set;
        }

        /// <summary>
        ///Default value: false.
        /// </summary>
        public bool CreateBucketIfNotExists
        {
            get; set;
        }
    }
}
