using System.Text.Json.Serialization;

namespace OnlineCurriculum.Models;

public class ResumeFile
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    [JsonIgnore]
    public CandidateProfile CandidateProfile { get; set; }
    public string FileName { get; set; }
    public string OriginalName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}