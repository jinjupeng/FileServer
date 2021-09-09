using FileServer.FileSystem;
using FileServer.ImageResize;
using FileServer.ImageResize.Utils;
using FileServer.Minio;
using FileServer.VirtualFileSystem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace FileServer.Hosted
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 256 MB
                options.MultipartBodyLengthLimit = 268435456;
            });
            //ע�� �����ȡHttpContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddVirtualFileServer(new List<FileServerOptions> {
                new FileServerOptions
                {
                    FileProvider = new PhysicalFileProvider("D:/UploadPath/FileSystem"),
                    RequestPath = new PathString(""),
                    EnableDirectoryBrowsing = false
                }
            }); 

            services.AddFileSystem(a =>
            {
                a.BasePath = "D:/UploadPath";
            });

            services.AddDynamicResizing(options => 
            {
                options.DefaultInstructions = new ResizeInstructions();
                options.MandatoryInstructions = new ResizeInstructions();
            });

            //services.AddMinio(options =>
            //{
            //    options.AccessKey = "";
            //    options.SecretKey = "";
            //    options.BucketName = "";
            //    options.EndPoint = "127.0.0.1:9000";
            //});

            #region Swagger UI

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API171",
                    Version = "v1",
                    Description = "����.NET Core 3.1 ��JWT �����֤",
                    Contact = new OpenApiContact
                    {
                        Name = "jinjupeng",
                        Email = "im.jp@outlook.com.com",
                        Url = new Uri("http://cnblogs.com/jinjupeng"),
                    },
                });
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                //{
                //    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            }
                //        },
                //        new string[] { }
                //    }
                //});
            });

            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IFileServerProvider fileServerProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDynamicResizing();

            // call convenience method which adds our FileServerOptions from the IFileServerProvider service
            app.UseFileServerProvider(fileServerProvider);

            app.UseRouting();

            app.UseAuthorization();

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //Ҫ��Ӧ�õĸ�(http://localhost:<port>/) ���ṩ Swagger UI���뽫 RoutePrefix ��������Ϊ���ַ���
                c.RoutePrefix = string.Empty;
                //swagger����auth��֤
            });

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
