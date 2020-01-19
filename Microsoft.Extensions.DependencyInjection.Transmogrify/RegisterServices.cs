using System;
using Transmogrify;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterServices
    {
        public static IServiceCollection AddCoreTransmogrify(this IServiceCollection services, Action<TransmogrifyConfig> configBuilder)
        {
            var config = new TransmogrifyConfig();
            configBuilder(config);

            services.AddSingleton(config);
            foreach (var resolver in config.LanguageResolvers)
            {
                services.AddScoped(typeof(ILanguageResolver),resolver);
            }
            
            services.AddScoped<ITranslator, Translator>();

            return services;
        }
    }
}