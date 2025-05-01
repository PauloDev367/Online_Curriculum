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

   
}