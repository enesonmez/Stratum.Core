using Core.Localization.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Localization.DB.EntityConfigurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources").HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
        builder.Property(x=>x.Key).HasColumnName("Key").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");
        builder.HasIndex(x=>x.Key).IsUnique();
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
        
        builder.HasMany(x => x.ResourceTranslations)
            .WithOne(x=> x.Resource)
            .HasForeignKey(x=>x.ResourceId);
        
        builder.HasBaseType((string)null!);
    }
}