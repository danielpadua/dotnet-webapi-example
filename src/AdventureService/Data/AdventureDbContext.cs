using System.Reflection;
using AdventureService.Data.Maps;
using AdventureService.Models;
using AdventureService.Utils;
using Microsoft.EntityFrameworkCore;

namespace AdventureService.Data
{
    public class AdventureDbContext : DbContext
    {
        public AdventureDbContext(DbContextOptions<AdventureDbContext> options) : base(options) { }

        public DbSet<Adventure> Adventures { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Adventure>(new AdventureMap().Configure);
            modelBuilder.Entity<Category>(new CategoryMap().Configure);
            modelBuilder.Entity<Tag>(new TagMap().Configure);
            modelBuilder.Entity<AdventureTag>(new AdventureTagMap().Configure);

            //modelBuilder.HasPostgresExtension("postgis");

            DatabaseUtils.UsePostgresNamingConventions(modelBuilder.Model.GetEntityTypes());
        }
    }

    public class AdventureContextDesignFactory : DesignTimeDbContextFactoryBase<AdventureDbContext>
    {
        public AdventureContextDesignFactory() : base("DefaultConnection", typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
        { }
        protected override AdventureDbContext CreateNewInstance(DbContextOptions<AdventureDbContext> options)
        {
            return new AdventureDbContext(options);
        }
    }


}