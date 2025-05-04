using OnlineCurriculum.Extensions;
using Serilog;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("serilog.json", optional: true, reloadOnChange: true);

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Host.AddSerilogLogging();
builder.Services.AddSwaggerWithJwtAuth();

builder.Services.AddControllers();

await builder.ConfigureS3();
builder.Services.ConfigureIdentityAuth(builder.Configuration);
builder.ConfigureDbContext();
builder.Services.ConfigureDependencies();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseGlobalExceptionHandler();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => { return "Service online"; });

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ConfigureRolesExtension.SeedRolesAsync(services);
}
try
{
    Log.Information("Starting OnlineCurriculum API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}