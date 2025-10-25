using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int ClaimMonth { get; set; } // 1-12
        public int ClaimYear { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal TotalAmount { get; set; }

        public string? Status { get; set; } = "Draft"; // Draft, Submitted, Reviewed, Approved, Rejected, Settled

        public string? Notes { get; set; }

        public DateTime DateSubmitted { get; set; }

        public ICollection<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();
        public ICollection<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
        public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }
}
