using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineCurriculum.Configurations;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;

namespace OnlineCurriculum.Services;

public class IdentityService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly JwtOptions _jwtOptions;

    public IdentityService(SignInManager<User> signInManager, UserManager<User> userManager,
        IOptions<JwtOptions> jwtOptions)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<ResponseRequest<SimpleUserResponseRequest>> CreateNewUser(CreateNewUserRequest request)
    {
        var identityUser = new User
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var response = new ResponseRequest<SimpleUserResponseRequest>();

        var result = await _userManager.CreateAsync(identityUser, request.Senha);
        if (result.Succeeded)
        {
            await _userManager.SetLockoutEnabledAsync(identityUser, false);
            response.Success = new SimpleUserResponseRequest(identityUser);
        }

        if (result.Errors.Count() > 0)
        {
            response.SetErros(result.Errors.Select(er => er.Description));
        }

        return response;
    }

    public async Task<ResponseRequest<object>> Login(UserLoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);

        var response = new ResponseRequest<object>();
        if (result.Succeeded)
        {
            var token = await GenerateToken(request.Email);
            response.Success = new { token };
        }

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                response.AddError("This account is locked out.");
            else if (result.IsNotAllowed)
                response.AddError("You are not allowed to access this action.");
            else if (result.RequiresTwoFactor)
                response.AddError("You need to confirm your 2FA verification code.");
            else
                response.AddError("E-mail or password invalid");
        }

        return response;
    }

    private async Task<string> GenerateToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var tokenClaims = await GetClaims(user);

        var expDate = DateTime.Now.AddSeconds(_jwtOptions.Expiration);
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: tokenClaims,
            notBefore: DateTime.Now,
            expires: expDate,
            signingCredentials: _jwtOptions.SigningCredentials
        );
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return token;
    }

    private async Task<IList<Claim>> GetClaims(User user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, unixTimestamp.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, unixTimestamp.ToString()));

        foreach (var role in roles)
            claims.Add(new Claim("role", role));

        return claims;
    }
}