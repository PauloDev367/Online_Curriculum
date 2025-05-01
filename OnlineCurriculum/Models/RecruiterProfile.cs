using System.Text.Json.Serialization;

namespace OnlineCurriculum.Models;

public class RecruiterProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [JsonIgnore] public User User { get; set; }
    public string CompanyName { get; set; }
    public string Position { get; set; }
    public string Departament { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}