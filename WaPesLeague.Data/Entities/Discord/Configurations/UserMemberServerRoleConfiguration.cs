using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class UserMemberServerRoleConfiguration : IEntityTypeConfiguration<UserMemberServerRole>
    {
        public void Configure(EntityTypeBuilder<UserMemberServerRole> builder)
        {
            builder.HasOne(umsr => umsr.UserMember)
                .WithMany(um => um.UserMemberServerRoles)
                .HasForeignKey(umsr => umsr.UserMemberId);

            builder.HasOne(umsr => umsr.ServerRole)
                .WithMany(sr => sr.UserMemberServerRoles)
                .HasForeignKey(umsr => umsr.ServerRoleId);
        }
    }
}
