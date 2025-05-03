using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    private readonly S3Service _s3Service;
    private readonly HttpClient _httpClient;

    public RecruiterService(AppDbContext dbContext, UserManager<User> userManager, S3Service s3Service, HttpClient httpClient)
    {
        _context = dbContext;
        _userManager = userManager;
        _s3Service = s3Service;
        _httpClient = httpClient;
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

    public async Task<FileStreamResult> GetCurriculumToDownload(string key)
    {
        var resumeFileExists = await _context.ResumeFiles
            .AsNoTracking()
            .Where(rf => rf.FileName.ToLower() == key.ToLower())
            .FirstOrDefaultAsync();

        if (resumeFileExists is null)
            throw new Exception("Curriculum not found");

        var presignedUrl = await _s3Service.GetFileFromBucketAsync(key);
        var response = await _httpClient.GetAsync(presignedUrl);
        if(!response.IsSuccessStatusCode)
            throw new Exception("Curriculum not found");
        
        var contentType = response.Content.Headers.ContentType?.ToString();
        var fileStream = await response.Content.ReadAsStreamAsync();
        var fileName = GetFileNameFromUrl(presignedUrl);

        return new FileStreamResult(fileStream, contentType)
        {
            FileDownloadName = fileName
        };
    }
    
    public async Task<PaginatedResultResponseRequest<CandidateProfile>> searchByCandidateAsync(
        int pageNumber, int pageSize, RecruiterCandidateSearchFiltersRequest request, Guid userId)
    {
        var query = _context.CandidateProfiles.AsQueryable();

        if (request.ExperienceYears > 0)
            query = query.Where(cp => cp.ExperienceYears >= request.ExperienceYears);

        if (!string.IsNullOrWhiteSpace(request.Technologies))
        {
            var techs = request.Technologies.Split(';',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var tech in techs)
            {
                var pattern = $"%{tech}%";
                query = query.Where(cp => EF.Functions.Like(cp.Technologies, pattern));
            }
        }

        var totalItems = await query.CountAsync();

        var data = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Where(cp => cp.UserId != userId)
            .ToListAsync();

        return new PaginatedResultResponseRequest<CandidateProfile>
        {
            Items = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
    
    private string GetFileNameFromUrl(string url)
    {
        return Path.GetFileName(new Uri(url).AbsolutePath);
    }
}