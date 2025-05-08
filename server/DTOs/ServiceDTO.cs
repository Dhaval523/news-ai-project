
namespace Server.DTOs;

public class ServiceDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public decimal TotalEarnings { get; set; }
    public int TotalJobsCompleted { get; set; }
    public int ExperienceInYears { get; set; }
    public double Rating { get; set; }
    public string Category { get; set; }
    public UserDTO Worker { get; set; }
}