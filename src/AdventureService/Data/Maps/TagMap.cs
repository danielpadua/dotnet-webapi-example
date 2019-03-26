using AdventureService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureService.Data.Maps
{
    public class TagMap : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tag");

            builder.HasKey(x => x.Name);

            builder.Property(x => x.Name)
                .HasMaxLength(20);
        }
    }
}