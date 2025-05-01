using Microsoft.AspNetCore.Mvc;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;
using OnlineCurriculum.Services;

namespace OnlineCurriculum.Controller;

[ApiController]
[Route("api/v1/candidate-profiles")]
public class CandidateProfileController : ControllerBase
{
    private readonly CandidateProfileService _service;

    public CandidateProfileController(CandidateProfileService candidateProfileService)
    {
        _service = candidateProfileService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateCandidateProfileRequest request)
    {
        try
        {
            var created = await _service.CreateAsync(request);
            return Ok(created);
        }
        catch (Exception e)
        {
            var responseRequest = new ResponseRequest<object>();
            if (e.Message.Equals("Failed to create user"))
            {
                responseRequest.AddError(e.Message);
                return BadRequest(responseRequest);
            }

            responseRequest.AddError(e.Message);
            return StatusCode(500, responseRequest);
        }
    }
}