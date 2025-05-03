namespace OnlineCurriculum.Requests;

public class CandidateProfileResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Bio { get; set; }
    public string Technologies { get; set; }
    public int ExperienceYears { get; set; }
    public string Location { get; set; }
    public Guid? ResumeFileId { get; set; }
}