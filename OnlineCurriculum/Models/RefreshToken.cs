namespace OnlineCurriculum.Models;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; }
    public string JwtId { get; set; } 
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; } = false;
    public bool Revoked { get; set; } = false;
    public User User { get; set; }
}