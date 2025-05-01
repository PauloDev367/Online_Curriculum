using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OnlineCurriculum.Configurations;
using OnlineCurriculum.Data;
using OnlineCurriculum.Models;

namespace OnlineCurriculum.Extensions;

public static class IdentityAuthConfigExtension
{
      public static void ConfigureIdentityAuth(this IServiceCollection service, IConfiguration configuration)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtOptions:SecurityKey").Value));
        var jwtAppSettingOptions = configuration.GetSection(nameof(JwtOptions));

        ConfigureJwtToken(service, configuration, securityKey, jwtAppSettingOptions);
        ConfigureIdentityUser(service);
        ConfigurePasswordRequirements(service);
    }

    private static void ConfigurePasswordRequirements(IServiceCollection service)
    {
        service.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;

        });
    }

    private static void ConfigureIdentityUser(IServiceCollection service)
    {
        service.AddDefaultIdentity<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void ConfigureJwtToken(IServiceCollection service, IConfiguration configuration, SymmetricSecurityKey securityKey, IConfigurationSection jwtAppSettingOptions)
    {
        service.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtAppSettingOptions[nameof(JwtOptions.Issuer)];
            options.Audience = jwtAppSettingOptions[nameof(JwtOptions.Audience)];
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            options.Expiration = int.Parse(jwtAppSettingOptions[nameof(JwtOptions.Expiration)] ?? "0");
        });

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration.GetSection("JwtOptions:Issuer").Value,

            ValidateAudience = true,
            ValidAudience = configuration.GetSection("JwtOptions:Audience").Value,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            ClockSkew = TimeSpan.Zero
        };

        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenValidationParameters;
        });
    }
}