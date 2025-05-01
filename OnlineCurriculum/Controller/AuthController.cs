using Microsoft.AspNetCore.Mvc;
using OnlineCurriculum.Requests;
using OnlineCurriculum.Services;

namespace OnlineCurriculum.Controller;
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IdentityService _identityService;

    public AuthController(IdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var token = await _identityService.Login(request);
    
        if (token.Errors != null && token.Errors.Any())
        {
            return BadRequest(token); // Retorna erro corretamente
        }

        return Ok(token); // Login bem-sucedido
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest model)
    {
        var data = await _identityService.Refresh(model);
        return Ok(data);
    }
}