using System.ComponentModel.DataAnnotations;

namespace OnlineCurriculum.Requests;

public class CreateCandidateProfileRequest
{
    [Required] public string FullName { get; set; }
    [Required] public string Bio { get; set; }
    [Required] public string Technologies { get; set; }
    [Required] public int ExperienceYears { get; set; }
    [Required] public string Location { get; set; }
}