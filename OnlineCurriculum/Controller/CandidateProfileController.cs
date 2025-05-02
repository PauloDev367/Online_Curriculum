using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCurriculum.Enums;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;
using OnlineCurriculum.Services;

namespace OnlineCurriculum.Controller;

[ApiController]
[Route("api/v1/candidate-profiles")]
[Authorize]
public class CandidateProfileController : ControllerBase
{
    private readonly CandidateProfileService _service;
    public CandidateProfileController(CandidateProfileService candidateProfileService)
    {
        _service = candidateProfileService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateProfile([FromBody] CreateCandidateProfileRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var profile = await _service.CreateOrUpdateProfileAsync(request, userId);
        return Ok(profile);
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.Candidate)]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var profile = await _service.GetProfileAsync(userId);
        if (profile == null)
            return NotFound("Profile not found.");

        return Ok(profile);
    }

    [HttpDelete]
    [Authorize(Roles = RoleConstants.Candidate)]
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var success = await _service.DeleteProfileAsync(userId);
        if (!success)
            return NotFound("Profile not found.");

        return NoContent();
    }

    // [HttpPut]
    // [Authorize(Roles = RoleConstants.Candidate)]
    // public async Task<IActionResult> UploadCurriculumProfile([FromForm] IFormFile file)
    // {
    //     
    //     return Ok();
    // }
    //
    
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
    
}