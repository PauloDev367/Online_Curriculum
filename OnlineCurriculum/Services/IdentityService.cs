using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineCurriculum.Configurations;
using OnlineCurriculum.Data;
using OnlineCurriculum.Models;
using OnlineCurriculum.Requests;

namespace OnlineCurriculum.Services;

public class IdentityService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly JwtOptions _jwtOptions;
    private readonly AppDbContext _context;

    public IdentityService(SignInManager<User> signInManager, UserManager<User> userManager,
        IOptions<JwtOptions> jwtOptions, AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
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

        var result = await _userManager.CreateAsync(identityUser, request.Password);
        if (result.Succeeded)
        {
            await _userManager.SetLockoutEnabledAsync(identityUser, false);
            response.Success = new SimpleUserResponseRequest(identityUser);
        }

        if (result.Errors.Count() > 0)
        {
            response.SetErros(result.Errors.Select(er => er.Description));
        }

        await _userManager.AddToRoleAsync(identityUser, request.Role.ToString());
        return response;
    }

    public async Task<ResponseRequest<object>> Login(UserLoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);

        var response = new ResponseRequest<object>();
        if (result.Succeeded)
        {
            var token = await GenerateToken(request.Email);
            var user = await _userManager.FindByEmailAsync(request.Email);
            var refreshToken = await GenerateRefreshToken(user, token.Jti);
            response.Success = new { token = token.Token, refreshToken };
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

    public async Task<object> Refresh(RefreshTokenRequest model)
    {
        var principal = GetPrincipalFromExpiredToken(model.Token);
        if (principal == null)
            throw new Exception("Invalid refresh token");

        var userId = principal.Claims.FirstOrDefault(x =>
            x.Type == JwtRegisteredClaimNames.Sub || x.Type == ClaimTypes.NameIdentifier)?.Value;
        var jti = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

        var tokens = await _context.RefreshTokens
            .Where(rt => rt.Token == model.RefreshToken)
            .ToListAsync();

        if (!tokens.Any())
            throw new Exception("Refresh token not found in DB");

        var storedToken = tokens.FirstOrDefault(rt =>
            rt.JwtId == jti &&
            rt.UserId == userId &&
            !rt.Used &&
            !rt.Revoked &&
            rt.ExpiresAt > DateTime.UtcNow);

        if (storedToken == null)
            throw new Exception("Refresh token found, but does not meet validation criteria");

        storedToken.Used = true;
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(userId);
        var newAccessToken = await GenerateToken(user.Email);
        var newRefreshToken = await GenerateRefreshToken(user, jti);

        return new
        {
            token = newAccessToken,
            refreshToken = newRefreshToken
        };
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = _jwtOptions.SigningCredentials.Key,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }


    private async Task<string> GenerateRefreshToken(User user, string jwtId)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString().Replace("-", ""),
            JwtId = jwtId,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken.Token;
    }


    private async Task<TokenResultResponse> GenerateToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var jti = Guid.NewGuid().ToString();
        var tokenClaims = await GetClaims(user, jti);

        var expDate = DateTime.Now.AddSeconds(_jwtOptions.Expiration);
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: tokenClaims,
            notBefore: DateTime.Now,
            expires: expDate,
            signingCredentials: _jwtOptions.SigningCredentials
        );

        return new TokenResultResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            Jti = jti
        };
    }


    private async Task<IList<Claim>> GetClaims(User user, string jti)
    {
        if (user == null || string.IsNullOrEmpty(user.Id))
            throw new Exception("User not found or ID is null when generating token.");
        
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti));
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id)); 
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, unixTimestamp.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, unixTimestamp.ToString()));

        foreach (var role in roles)
            claims.Add(new Claim("role", role));

        return claims;
    }
}