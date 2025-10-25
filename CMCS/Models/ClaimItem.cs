using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class ClaimItem
    {
        public int ClaimItemId { get; set; }

        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal HoursWorked { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal HourlyRate { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal Amount { get; set; }
    }
}
