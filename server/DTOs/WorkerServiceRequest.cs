using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class WorkerServiceRequest
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int ExperienceInYears { get; set; } = 0;

        [MaxLength(100)]
        public string? Category { get; set; }
    }
}
