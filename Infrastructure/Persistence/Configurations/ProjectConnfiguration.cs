using Application.Constants;

namespace Infrastructure.Persistence.Configurations;


public class ProjectConnfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Description).HasDefaultValue("").HasMaxLength(255);
        
        builder.HasOne(x => x.Group).WithMany(x => x.Projects);
    }
}