using System.ComponentModel.DataAnnotations;

namespace OnlineCurriculum.Requests;

public class CreateCandidateProfileRequest
{
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Bio { get; set; }
    [Required]
    public string Technilogies { get; set; }
    [Required]
    public int ExperienceYears { get; set; }
    [Required]
    public string Location { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(50)]
    public string Password { get; set; }
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}