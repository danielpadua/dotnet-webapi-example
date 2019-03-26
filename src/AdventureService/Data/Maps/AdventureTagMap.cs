using AdventureService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureService.Data.Maps
{
    public class AdventureTagMap : IEntityTypeConfiguration<AdventureTag>
    {
        public void Configure(EntityTypeBuilder<AdventureTag> builder)
        {
            builder.ToTable("adventure_tag");

            builder.HasKey(x => new { x.AdventureId, x.TagName });

            builder.HasOne(x => x.Adventure)
                .WithMany(y => y.AdventureTags)
                .HasForeignKey(x => x.AdventureId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tag)
                .WithMany(y => y.AdventureTags)
                .HasForeignKey(x => x.TagName)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}