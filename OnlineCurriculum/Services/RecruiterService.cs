using OnlineCurriculum.Data;
using OnlineCurriculum.Enums;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;

namespace OnlineCurriculum.Services;

public class RecruiterService
{
    private readonly IdentityService _identityService;
    private readonly AppDbContext _dbContext;

    public RecruiterService(IdentityService identityService, AppDbContext dbContext)
    {
        _identityService = identityService;
        _dbContext = dbContext;
    }

    public async Task<ResponseRequest<RecruiterProfile>> CreateAsync(CreateRecruiterProfileRequest request)
    {
        var createUserRequest = new CreateNewUserRequest
        {
            Email = request.Email,
            Name = request.FullName,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            Role = Role.Recruiter
        };
        var createdUser = await _identityService.CreateNewUser(createUserRequest);

        if (createdUser.Errors.Count > 0)
        {
            throw new Exception("Failed to create user");
        }

        var candidate = new RecruiterProfile
        {
            CompanyName = request.CompanyName,
            Departament = request.Departament,
            Position = request.Position,
            UserId = Guid.Parse(createdUser.Success.Id)
        };

        await _dbContext.AddAsync(candidate);
        await _dbContext.SaveChangesAsync();
        var response = new ResponseRequest<RecruiterProfile>
        {
            Errors = [],
            Success = candidate
        };

        return response;
    }
}