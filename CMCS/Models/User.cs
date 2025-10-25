using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(150), EmailAddress]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Role { get; set; } // Lecturer, Coordinator, Manager
    }
}
