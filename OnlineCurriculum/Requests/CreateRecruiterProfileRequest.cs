using System.ComponentModel.DataAnnotations;

namespace OnlineCurriculum.Requests;

public class CreateRecruiterProfileRequest
{
    [Required] public string CompanyName { get; set; }
    [Required] public string Position { get; set; }
    [Required] public string Departament { get; set; }
}