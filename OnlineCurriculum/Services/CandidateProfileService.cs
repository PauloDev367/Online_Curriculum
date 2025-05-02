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
    private readonly S3Service _s3Service;

    public CandidateProfileService(AppDbContext context, UserManager<User> userManager, S3Service s3Service)
    {
        _context = context;
        _userManager = userManager;
        _s3Service = s3Service;
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

    public async Task<CreatedBucketResponse> UploadResumeFileAsync(Guid userId, IFormFile file)
    {
        var profile = await _context.CandidateProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile is null)
            throw new Exception("Candidate profile not found");
        
        var resumeExists = await _context.ResumeFiles.FirstOrDefaultAsync(rf =>rf.CandidateId == profile.Id);
        if(resumeExists != null)
            throw new Exception("Resume file already exists");
        
        var upload = await _s3Service.UploadFileAsync(file);
        var resumeFile = new ResumeFile
        {
            CandidateId = profile.Id,
            ContentType = upload.ContentType,
            FileName = upload.FileName,
            FileSize = upload.FileSize,
            OriginalName = upload.OriginalName,
        };
        
        await _context.ResumeFiles.AddAsync(resumeFile);
        await _context.SaveChangesAsync();
        return upload;
    }
}