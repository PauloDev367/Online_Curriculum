using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace OnlineCurriculum.Models;

public class User : IdentityUser
{
    [JsonIgnore] public Guid? CandidateProfileId { get; set; }
    [JsonIgnore] public CandidateProfile? CandidateProfile { get; set; }
    [JsonIgnore] public Guid? RecruiterProfileId { get; set; }
    [JsonIgnore] public RecruiterProfile? RecruiterProfile { get; set; }
}