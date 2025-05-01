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
}