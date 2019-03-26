using System;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AdventureService.Data
{
    public static class DatabaseInitializer
    {
        private const string CODELAB_ADMIN = "codelab_admin";

        public static void InitializeDatabase(IServiceProvider services)
        {
            PerformMigrations(services);
            SeedData(services);
        }

        private static void PerformMigrations(IServiceProvider services)
        {
            // TODO: Remove when production
            //services.GetService<AdventureDbContext>().Database.EnsureDeleted();

            Log.Information("Perform database migrations if needed...");
            services.GetService<AdventureDbContext>().Database.Migrate();
            services.GetService<AdventureDbContext>().Database.EnsureCreated();
        }
        private static void SeedData(IServiceProvider services)
        {

        }
    }
}