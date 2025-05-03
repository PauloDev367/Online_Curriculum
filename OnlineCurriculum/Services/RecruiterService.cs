using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineCurriculum.Data;
using OnlineCurriculum.Enums;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;

namespace OnlineCurriculum.Services;

public class RecruiterService
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    public RecruiterService(AppDbContext dbContext, UserManager<User> userManager)
    {
        _context = dbContext;
        _userManager = userManager;
    }
    
    public async Task<RecruiterProfile> CreateOrUpdateProfileAsync(Guid userId, CreateRecruiterProfileRequest dto)
    {
        var existingProfile = await _context.RecruiterProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (existingProfile is null)
        {
            var newProfile = new RecruiterProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyName = dto.CompanyName,
                Position = dto.Position,
                Departament = dto.Departament,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.AddToRoleAsync(user, RoleConstants.Recruiter);
            
            _context.RecruiterProfiles.Add(newProfile);
            await _context.SaveChangesAsync();
            return newProfile;
        }
        else
        {
            existingProfile.CompanyName = dto.CompanyName;
            existingProfile.Position = dto.Position;
            existingProfile.Departament = dto.Departament;
            existingProfile.UpdatedAt = DateTime.UtcNow;

            _context.RecruiterProfiles.Update(existingProfile);
            await _context.SaveChangesAsync();
            return existingProfile;
        }
    }

    public async Task<RecruiterProfile?> GetProfileAsync(Guid userId)
    {
        return await _context.RecruiterProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> DeleteProfileAsync(Guid userId)
    {
        var profile = await _context.RecruiterProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        await _userManager.RemoveFromRoleAsync(user, RoleConstants.Recruiter);
        
        _context.RecruiterProfiles.Remove(profile);
        await _context.SaveChangesAsync();
        return true;
    }
    
}