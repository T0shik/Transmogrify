using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Transmogrify;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Transmogrify;

namespace Sample
{
    public class LanguageResolver : ILanguageResolver
    {
        private readonly HttpContext _httpContext;

        public LanguageResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Task<string> GetLanguageCode()
        {
            var header = _httpContext.Request.Headers["Accept-Language"];
            return Task.FromResult(header.FirstOrDefault());
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITransmogrifyJson, TransmogrifyJson>();
            
            services.AddHttpContextAccessor();
            services.AddScoped<ILanguageResolver, LanguageResolver>();
            var config = new Config
            {
                LanguagePacks =
                {
                    ["en"] = "./Samples/english.json",
                    ["ru"] = "./Samples/russian.json"
                },
            };
            services.AddSingleton(config);
            services.AddScoped<ITranslator, Translator>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}