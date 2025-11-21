using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Web.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
