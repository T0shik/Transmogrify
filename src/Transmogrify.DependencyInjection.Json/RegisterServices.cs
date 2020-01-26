using System;
using System.Text.Json.Transmogrify;
using Transmogrify;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterServices
    {
        public static IServiceCollection AddTransmogrify(
            this IServiceCollection services,
            Action<TransmogrifyConfig> configBuilder)
        {
            services.AddSingleton<ITransmogrifyJson, TransmogrifyJson>();

            return services.AddCoreTransmogrify(configBuilder);
        }
    }
}