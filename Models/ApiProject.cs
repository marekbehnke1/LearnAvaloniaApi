using System.ComponentModel.DataAnnotations;

namespace LearnAvaloniaApi.Models
{
    public class ApiProject
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }

        // Navigation properties
        public virtual ICollection<ApiTask> Tasks { get; set; } = new List<ApiTask>();

    }
}
