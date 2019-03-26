using AdventureService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureService.Data.Maps
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("category");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.HasMany(x => x.Adventures)
                .WithOne(y => y.Category)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}