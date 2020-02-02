using System;
using Microsoft.Extensions.DependencyInjection;

namespace Transmogrify.DependencyInjection.Json
{
    public static class RegisterServices
    {
        public static IServiceCollection AddTransmogrify(
            this IServiceCollection services,
            Action<TransmogrifyConfig> configBuilder)
        {
            services.AddSingleton<ILibraryFactory, LibraryFactory>();

            return services.AddCoreTransmogrify(configBuilder);
        }
    }
}