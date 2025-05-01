namespace OnlineCurriculum.Requests;

public class SimpleUserResponseRequest
{
    public string Id { get; set; }
    public string Email { get; set; }
    public SimpleUserResponseRequest(Models.User identityUser)
    {
        Id = identityUser.Id;
        Email = identityUser.Email;
    }
}