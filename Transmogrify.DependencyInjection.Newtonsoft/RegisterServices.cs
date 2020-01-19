using System;
using Microsoft.Extensions.DependencyInjection.Transmogrify.Newtonsoft;
using Transmogrify;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterServices
    {
        public static IServiceCollection AddNewtonsoftTransmogrify(
            this IServiceCollection services,
            Action<TransmogrifyConfig> configBuilder)
        {
            services.AddSingleton<ITransmogrifyJson, TransmogrifyJson>();

            return services.AddCoreTransmogrify(configBuilder);
        }
    }
}