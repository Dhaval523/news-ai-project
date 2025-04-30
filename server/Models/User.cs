using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; }

    [MaxLength(100)]
    public string FirstName { get; set; }

    [MaxLength(100)]
    public string LastName { get; set; }

    [MaxLength(255)]
    public string ProfileImageUrl { get; set; }

    public string Bio { get; set; }

    [MaxLength(100)]
    public string Location { get; set; }

    public bool IsVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLogin { get; set; }

    // Using System.Text.Json for JSON handling
    public Dictionary<string, object> Preferences { get; set; } = new Dictionary<string, object>();

    // Navigation properties
    // public ICollection<UserSavedArticle> SavedArticles { get; set; }
    // public ICollection<Comment> Comments { get; set; }
    // public ICollection<Notification> Notifications { get; set; }
}
}