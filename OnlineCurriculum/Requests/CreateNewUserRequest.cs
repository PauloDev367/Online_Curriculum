using System.ComponentModel.DataAnnotations;
using OnlineCurriculum.Enums;

namespace OnlineCurriculum.Requests;

public class CreateNewUserRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(50)]
    public string Password { get; set; }
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    public Role Role { get; set; }
}