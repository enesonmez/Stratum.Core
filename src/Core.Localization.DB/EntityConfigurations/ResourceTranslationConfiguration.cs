using Core.Localization.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Localization.DB.EntityConfigurations;

public class ResourceTranslationConfiguration : IEntityTypeConfiguration<ResourceTranslation>
{
    public void Configure(EntityTypeBuilder<ResourceTranslation> builder)
    {
        builder.ToTable("ResourceTranslations").HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
        builder.Property(x=>x.CultureCode).HasColumnName("CultureCode").IsRequired();
        builder.Property(x=>x.Value).HasColumnName("Value").IsRequired();
        builder.Property(x=>x.ResourceId).HasColumnName("ResourceId").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
        
        builder.HasOne(x => x.Resource)
            .WithMany(x=>x.ResourceTranslations)
            .HasForeignKey(x => x.ResourceId);
        
        builder.HasBaseType((string)null!);
    }
}