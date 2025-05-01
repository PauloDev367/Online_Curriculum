using Microsoft.AspNetCore.Mvc;
using OnlineCurriculum.Requests;
using OnlineCurriculum.Services;

namespace OnlineCurriculum.Controller;

[ApiController]
[Route("api/recruiter-profiles")]
public class RecruiterProfileController : ControllerBase
{
    private readonly RecruiterService _service;

    public RecruiterProfileController(RecruiterService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateRecruiterProfileRequest request)
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