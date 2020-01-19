using System;
using System.Collections.Generic;
using System.Text.Json.Transmogrify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.LanguageResolvers;

namespace Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddTransmogrify(config =>
            {
                config.LanguagePacks.Add("en", "./Samples/english.json");
                config.LanguagePacks.Add("ru", "./Samples/russian.json");
                config.AddResolver(typeof(QueryLanguageResolver));
                config.AddResolver(typeof(HeaderLanguageResolver));
            });
            
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