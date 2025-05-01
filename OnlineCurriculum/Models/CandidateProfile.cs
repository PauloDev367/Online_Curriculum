using System.Text.Json.Serialization;

namespace OnlineCurriculum.Models;

public class CandidateProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [JsonIgnore] public User User { get; set; }
    public string FullName { get; set; }
    public string Bio { get; set; }
    public string Technilogies { get; set; }
    public int ExperienceYears { get; set; }
    public string Location { get; set; }
    public Guid ResumeFileId { get; set; }
    [JsonIgnore] public ResumeFile ResumeFile { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}