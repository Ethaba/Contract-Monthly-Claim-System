using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class ClaimItem
    {
        public int ClaimItemId { get; set; }

        [Required]
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        [Range(0, 1000, ErrorMessage = "Hours must be >= 0.")]
        public decimal HoursWorked { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        [Range(0, 100000, ErrorMessage = "Hourly rate must be >= 0.")]
        public decimal HourlyRate { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal Amount { get; set; } // computed server-side: HoursWorked * HourlyRate
    }
}
