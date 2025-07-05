using System.ComponentModel.DataAnnotations;

namespace LearnAvaloniaApi.Models
{
    public class ApiProject
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // this might break stuff till i get user things sorted..
        [Required]
        public int UserId { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ApiTask> Tasks { get; set; } = new List<ApiTask>();

    }
}
