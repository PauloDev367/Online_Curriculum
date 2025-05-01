using System.ComponentModel.DataAnnotations;

namespace OnlineCurriculum.Requests;

public class CreateRecruiterProfileRequest
{
    [Required] public string FullName { get; set; }
    [Required] public string CompanyName { get; set; }
    [Required] public string Position { get; set; }
    [Required] public string Departament { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(50)]
    public string Password { get; set; }
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}