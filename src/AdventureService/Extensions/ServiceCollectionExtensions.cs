using System;
using System.IO;
using System.Reflection;
using AdventureService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AdventureService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAdventureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AdventureDbContext>(x =>
            {
                x.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
            });
        }
        public static void AddAdventureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Addventure Adventures API",
                    Description = "Addventures's API for managing adventures",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Daniel De Padua Ferreira",
                        Email = "daniel.padua@outlook.com",
                        Url = "https://www.linkedin.com/in/danielpadua"
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}