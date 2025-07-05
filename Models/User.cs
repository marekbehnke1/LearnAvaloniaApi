using System.ComponentModel.DataAnnotations;

namespace LearnAvaloniaApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)] // BCrypt hashes are ~60 chars, but give room for future algorithms
        public string PasswordHash { get; set; } = string.Empty;

        // Security & Auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        // Account Management
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;

        // Security tracking
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        // Navigation properties for EF Core relationships
        public virtual ICollection<ApiProject> Projects { get; set; } = new List<ApiProject>();
        public virtual ICollection<ApiTask> Tasks { get; set; } = new List<ApiTask>();
    }
}