namespace Infrastructure.Persistence.Configurations;


public class GroupMembershipConfiguration : IEntityTypeConfiguration<GroupMembership>
{
    public void Configure(EntityTypeBuilder<GroupMembership> builder)
    {
        builder.ToTable("GroupMemberships");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.GroupId).IsRequired();
        builder.Property(x => x.GroupRoleId).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.GroupMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Memberships)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.GroupRole)
            .WithMany()
            .HasForeignKey(x => x.GroupRoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.GroupId }).IsUnique();
    }
}