using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCurriculum.Data.Configurations;
using OnlineCurriculum.Models;

namespace OnlineCurriculum.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<CandidateProfile> CandidateProfiles { get; set; }
    public DbSet<RecruiterProfile> RecruiterProfiles { get; set; }
    public DbSet<ResumeFile> ResumeFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new CandidateProfileConfiguration());
        builder.ApplyConfiguration(new RecruiterProfileConfiguration());
        builder.ApplyConfiguration(new ResumeFileConfiguration());
    }
}