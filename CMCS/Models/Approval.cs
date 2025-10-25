using System;

namespace CMCS.Models
{
    public class Approval
    {
        public int ApprovalId { get; set; }

        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        public int ApproverUserId { get; set; }
        public User? Approver { get; set; }

        public string? Role { get; set; } // Coordinator / Manager
        public string? Decision { get; set; } // Approved / Rejected / Pending
        public DateTime DecisionAt { get; set; } = DateTime.UtcNow;

        public string? Comments { get; set; }
    }
}
