using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCurriculum.Models;

namespace OnlineCurriculum.Data.Configurations;

public class ResumeFileConfiguration : IEntityTypeConfiguration<ResumeFile>
{
    public void Configure(EntityTypeBuilder<ResumeFile> builder)
    {
        builder.HasOne(r=>r.CandidateProfile);
        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}