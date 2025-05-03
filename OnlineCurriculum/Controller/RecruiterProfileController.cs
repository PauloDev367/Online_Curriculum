using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCurriculum.Enums;
using OnlineCurriculum.Requests;
using OnlineCurriculum.Services;

namespace OnlineCurriculum.Controller;

[ApiController]
[Route("api/v1/recruiter-profiles")]
[Authorize]
public class RecruiterProfileController : ControllerBase
{
    private readonly RecruiterService _service;

    public RecruiterProfileController(RecruiterService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateProfile([FromBody] CreateRecruiterProfileRequest dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var profile = await _service.CreateOrUpdateProfileAsync(userId, dto);
        return Ok(profile);
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.Recruiter)]
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
    [Authorize(Roles = RoleConstants.Recruiter)]
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

    [HttpGet("candidates")]
    [Authorize(Roles = RoleConstants.Recruiter)]
    public async Task<IActionResult> GetRecruiterProfile(
        [FromQuery] RecruiterCandidateSearchFiltersRequest request,
        [FromQuery] int perPage = 10,
        [FromQuery] int page = 1)
    {
        var userId = GetUserId();
        var data = await _service
            .searchByCandidateAsync(page, perPage, request, userId);

        return Ok(data);
    }

    [HttpGet("candidates/resume-file/{key}")]
    [Authorize(Roles = RoleConstants.Recruiter)]
    public async Task<IActionResult> GetResumeFile([FromRoute] string key)
    {
        var resumeFile = await _service.GetCurriculumToDownload(key);
        return resumeFile is not null ? resumeFile : NotFound("Curriculum not found.");
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet("candidates/{id}")]
    [Authorize(Roles = RoleConstants.Recruiter)]
    public async Task<IActionResult> GetCandidateProfile([FromRoute] Guid id)
    {
        var data = await _service.GetCandidateProfileAsync(id);
        return Ok(data);
    }
}