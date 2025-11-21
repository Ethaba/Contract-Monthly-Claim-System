using System.ComponentModel.DataAnnotations;

namespace CMCS.Web.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ClaimMonth { get; set; }
        public int ClaimYear { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        public List<ClaimItem> ClaimItems { get; set; } = new();
        public List<SupportingDocument> SupportingDocuments { get; set; } = new();
        public List<Approval> Approvals { get; set; } = new();
    }
}
