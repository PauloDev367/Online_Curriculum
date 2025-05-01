using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineCurriculum.Data;
using OnlineCurriculum.Enums;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;

namespace OnlineCurriculum.Services;

public class CandidateProfileService
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public CandidateProfileService(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<CandidateProfile> CreateOrUpdateProfileAsync(CreateCandidateProfileRequest request, Guid userId)
    {
        var existingProfile = await _context.CandidateProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (existingProfile is null)
        {
            var newProfile = new CandidateProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FullName = request.FullName,
                Bio = request.Bio,
                Technilogies = request.Technilogies,
                ExperienceYears = request.ExperienceYears,
                Location = request.Location,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.AddToRoleAsync(user, RoleConstants.Candidate);

            _context.CandidateProfiles.Add(newProfile);
            await _context.SaveChangesAsync();
            return newProfile;
        }
        else
        {
            existingProfile.FullName = request.FullName;
            existingProfile.Bio = request.Bio;
            existingProfile.Technilogies = request.Technilogies;
            existingProfile.ExperienceYears = request.ExperienceYears;
            existingProfile.Location = request.Location;
            existingProfile.UpdatedAt = DateTime.UtcNow;

            _context.CandidateProfiles.Update(existingProfile);
            await _context.SaveChangesAsync();
            return existingProfile;
        }
    }

    public async Task<CandidateProfile?> GetProfileAsync(Guid userId)
    {
        return await _context.CandidateProfiles
            .Include(p => p.ResumeFile)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> DeleteProfileAsync(Guid userId)
    {
        var profile = await _context.CandidateProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile is null)
            return false;

        
        var user = await _userManager.FindByIdAsync(userId.ToString());
        await _userManager.RemoveFromRoleAsync(user, RoleConstants.Candidate);
        
        _context.CandidateProfiles.Remove(profile);
        await _context.SaveChangesAsync();

        return true;
    }
}