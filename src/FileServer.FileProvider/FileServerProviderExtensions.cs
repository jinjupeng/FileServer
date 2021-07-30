﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;

namespace FileServer.FileProvider
{
    /// <summary>
    /// Wrapper for UseFileServer to easily use the FileServerOptions registered in the IFileServerProvider
    /// </summary>
    public static class FileServerProviderExtensions
    {
        public static IServiceCollection AddFileServer(this IServiceCollection services, IConfiguration cfg)
        {
            // Add our IFileServerProvider implementation as a singleton
            services.AddSingleton<IFileServerProvider>(new FileServerProvider(
                new List<FileServerOptions>
                {
                    new FileServerOptions
                    {
                        FileProvider = new PhysicalFileProvider(cfg["FileServer:UploadPath"]),
                        RequestPath = new PathString(cfg["FileServer:RequestPath"]),
                        EnableDirectoryBrowsing = Convert.ToBoolean(cfg["FileServer:EnableDirectoryBrowsing"]),
                    },
                    //new FileServerOptions
                    //{
                    //    FileProvider = new PhysicalFileProvider(@"\\server\path"),
                    //    RequestPath = new PathString("/MyPath"),
                    //    EnableDirectoryBrowsing = true
                    //}
                }));

            return services;
        }
        public static IApplicationBuilder UseFileServerProvider(this IApplicationBuilder application, IFileServerProvider fileServerprovider)
        {
            foreach (var option in fileServerprovider.FileServerOptionsCollection)
            {
                application.UseFileServer(option);
            }
            return application;
        }
    }
}
