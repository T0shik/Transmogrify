using System;
using Microsoft.Extensions.DependencyInjection;

namespace Transmogrify.DependencyInjection.Newtonsoft
{
    public static class RegisterServices
    {
        public static IServiceCollection AddNewtonsoftTransmogrify(
            this IServiceCollection services,
            Action<TransmogrifyConfig> configBuilder)
        {
            services.AddSingleton<ILibraryFactory, LibraryFactory>();

            return services.AddCoreTransmogrify(configBuilder);
        }
    }
}