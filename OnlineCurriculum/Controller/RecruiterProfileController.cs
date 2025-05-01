using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCurriculum.Enums;
using OnlineCurriculum.Requests;
using OnlineCurriculum.Services;

namespace OnlineCurriculum.Controller;

[ApiController]
[Route("api/v1/recruiter-profiles")]
public class RecruiterProfileController : ControllerBase
{
    private readonly RecruiterService _service;

    public RecruiterProfileController(RecruiterService service)
    {
        _service = service;
    }

   
}