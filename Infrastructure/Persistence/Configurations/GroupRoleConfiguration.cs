using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations;

public class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
{
    public void Configure(EntityTypeBuilder<GroupRole> builder)
    {
        builder.ToTable("GroupRoles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.IsDefault).IsRequired();

        builder.HasOne(x => x.Group)
            .WithMany()
            .HasForeignKey(x => x.GroupId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Permissions)
               .HasColumnType("json");
    }
}