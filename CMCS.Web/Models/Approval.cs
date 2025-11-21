using System.ComponentModel.DataAnnotations;

namespace CMCS.Web.Models
{
    public class Approval
    {
        [Key]
        public int ApprovalId { get; set; }
        public int ClaimId { get; set; }
        public int ApproverId { get; set; }
        public User? Approver { get; set; }
        public string Role { get; set; } = null!;
        public string Decision { get; set; } = null!;
        public string? Comments { get; set; }
        public DateTime DecisionAt { get; set; } = DateTime.UtcNow;
    }
}
