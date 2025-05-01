using OnlineCurriculum.Data;
using OnlineCurriculum.Enums;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;

namespace OnlineCurriculum.Services;

public class CandidateProfileService
{
    private readonly IdentityService _identityService;
    private readonly AppDbContext _dbContext;

    public CandidateProfileService(IdentityService identityService, AppDbContext dbContext)
    {
        _identityService = identityService;
        _dbContext = dbContext;
    }

    public async Task<ResponseRequest<CandidateProfile>> CreateAsync(CreateCandidateProfileRequest request)
    {
        var createUserRequest = new CreateNewUserRequest
        {
            Email = request.Email,
            Name = request.FullName,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            Role = Role.Candidate
        }; 
        var createdUser = await _identityService.CreateNewUser(createUserRequest);
        
        if(createdUser.Errors.Count > 0)
        {
            throw new Exception("Failed to create user");
        }

        var candidate = new CandidateProfile
        {
            FullName = request.FullName,
            Bio = request.Bio,  
            Technilogies = request.Technilogies,
            ExperienceYears = request.ExperienceYears,
            Location =  request.Location,
            UserId = Guid.Parse(createdUser.Success.Id)
        };
        
        await _dbContext.AddAsync(candidate);
        await _dbContext.SaveChangesAsync();
        var response = new ResponseRequest<CandidateProfile>
        {
            Errors = [],
            Success = candidate
        };
        
        return response;
    }
}