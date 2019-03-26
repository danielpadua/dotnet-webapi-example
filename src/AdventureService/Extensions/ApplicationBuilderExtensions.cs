using System.Linq;
using Microsoft.AspNetCore.Builder;

namespace AdventureService.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseAdventureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((document, request) =>
                {
                    document.Paths = document.Paths.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value);
                });
            }).UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Addventure Adventures API v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}