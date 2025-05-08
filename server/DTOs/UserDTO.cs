namespace Server.DTOs;

public class UserDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public string Role { get; set; }
    public string? Location { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, object> Preferences { get; set; } = new Dictionary<string, object>();// If Preferences is a JSON or string property
}
