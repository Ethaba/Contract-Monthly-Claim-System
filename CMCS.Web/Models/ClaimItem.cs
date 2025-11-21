using System.ComponentModel.DataAnnotations;

namespace CMCS.Web.Models
{
    public class ClaimItem
    {
        [Key]
        public int ClaimItemId { get; set; }
        public int ClaimId { get; set; }
        public string? Description { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal Amount { get; set; }
    }
}
