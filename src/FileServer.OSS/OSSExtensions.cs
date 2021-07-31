using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileServer.OSS
{
    public static class OSSExtensions
    {
        public static IServiceCollection AddOSSProvider(this IServiceCollection services, IConfiguration cfg)
        {
            var option = new OSSOptions();
            var section = cfg.GetSection("FileServer");
            if (section == null)
            {
                throw new ArgumentNullException(nameof(OSSOptions));
            }

            section.Bind(option);

            services.AddSingleton(option);
            services.Configure<OSSOptions>(section);

            if (option.ProviderType == (int)ProviderType.Local)
            {
                services.AddSingleton<IOSSProvider, LocalOSSProvider>();
            }
            else if(option.ProviderType == (int)ProviderType.Aliyun)
            {
                services.AddSingleton<IOSSProvider, AliyunOSSProvider>();
            }

            return services;
        }
    }
}
