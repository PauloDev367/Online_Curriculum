using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCurriculum.Models;

namespace OnlineCurriculum.Data.Configurations;

public class RecruiterProfileConfiguration : IEntityTypeConfiguration<RecruiterProfile>
{
    public void Configure(EntityTypeBuilder<RecruiterProfile> builder)
    {
        builder.HasOne(r => r.User)
            .WithOne(u=>u.RecruiterProfile);
        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}