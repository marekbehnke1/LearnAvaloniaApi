using System.ComponentModel.DataAnnotations;

namespace LearnAvaloniaApi.Models
{
    public class ApiTask
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        public int Priority { get; set; } = 0;

        [Required]
        public bool IsCollapsed { get; set; } = false;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }
        public int? ProjectId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ApiProject? Project { get; set; }
    }
}
