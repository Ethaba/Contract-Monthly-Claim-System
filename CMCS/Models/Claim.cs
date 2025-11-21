using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int ClaimMonth { get; set; } // 1-12

        [Range(2000, 2100, ErrorMessage = "Enter a valid year.")]
        public int ClaimYear { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal TotalAmount { get; set; }

        public string? Status { get; set; } = "Draft"; // Draft, Submitted, Reviewed, Approved, Rejected, Settled

        public string? Notes { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        public ICollection<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();
        public ICollection<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
        public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }
}
