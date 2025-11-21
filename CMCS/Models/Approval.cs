using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Approval
    {
        public int ApprovalId { get; set; }

        [Required]
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        [Required]
        public int ApproverUserId { get; set; }
        public User? Approver { get; set; }

        [Required]
        [StringLength(50)]
        public string? Role { get; set; } // e.g., Coordinator, Manager, HR

        [Required]
        [StringLength(20)]
        public string? Decision { get; set; } = "Pending"; // Pending / Approved / Rejected

        public DateTime DecisionAt { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Comments { get; set; }
    }
}
