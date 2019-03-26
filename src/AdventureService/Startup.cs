using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdventureService.Extensions;
using AdventureService.Infraestructure;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AdventureService
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
            services.AddAdventureDatabase(Configuration);

            services.AddAdventureSwagger(Configuration);

            services.AddAutoMapper();

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddMvc(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddJsonOptions(jo =>
                {
                    jo.SerializerSettings.Converters.Add(new StringEnumConverter());
                    jo.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    jo.SerializerSettings.ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddViewLocalization()
                .AddControllersAsServices();

            // Localization                    
            services.AddJsonLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("pt-BR"),
                    new CultureInfo("en-US")
                };
                options.DefaultRequestCulture = new RequestCulture("pt-BR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAdventureSwagger();

            app.ConfigureExceptionHandler(loggerFactory.CreateLogger("exception"));

            // Localization
            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}