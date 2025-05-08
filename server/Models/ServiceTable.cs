using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Models ;

public class ServiceTable
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int WorkerId { get; set; }  

    [ForeignKey("WorkerId")]
    public virtual User Worker { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }  

    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [MaxLength(255)]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Range(0, 5)]
    public double Rating { get; set; } = 0;

    public int TotalJobsCompleted { get; set; } = 0;

    public decimal TotalEarnings { get; set; } = 0;

    public int ExperienceInYears { get; set; } = 0;

    [MaxLength(100)]
    public string? Category { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
