﻿using System.ComponentModel.DataAnnotations;

namespace LearnAvaloniaApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get;set; } = string.Empty;
        [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin {  get; set; }
    }
}
