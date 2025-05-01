using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCurriculum.Models;

namespace OnlineCurriculum.Data.Configurations;

public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
{
    public void Configure(EntityTypeBuilder<CandidateProfile> builder)
    {
        builder.HasOne(c => c.User)
            .WithOne(u=>u.CandidateProfile);
    
        builder.HasOne(c => c.ResumeFile)
            .WithOne(r=>r.CandidateProfile)
            .IsRequired(false);
        
        builder.Property(c => c.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}