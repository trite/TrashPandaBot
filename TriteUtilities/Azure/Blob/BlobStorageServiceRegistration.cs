using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriteUtilities.Azure.Blob
{
    public static class BlobStorageServiceRegistration
    {
        public static IServiceCollection RegisterStorageService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<BlobStorageOptions>(config.GetSection(BlobStorageOptions.configLocation));

            services.AddSingleton<BlobStorageService>();

            return services;
        }
    }
}
