using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Configurations;


public class GroupConnfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Description).HasDefaultValue("").HasMaxLength(255);
    }
}