using System;
using AdventureService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdventureService.Data.Maps
{
    public class AdventureMap : IEntityTypeConfiguration<Adventure>
    {
        public void Configure(EntityTypeBuilder<Adventure> builder)
        {
            builder.ToTable("addventure");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MainPhotoUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.Location)
                .IsRequired();

            builder.Property(x => x.Level)
                .IsRequired()
                .HasConversion(new EnumToStringConverter<Level>())
                .HasMaxLength(15);

            builder.HasOne(x => x.Category)
                .WithMany(y => y.Adventures)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Rating)
                .HasFilter("rating between 0 and 5");
        }
    }
}