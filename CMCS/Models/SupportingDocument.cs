using System;

namespace CMCS.Models
{
    public class SupportingDocument
    {
        public int SupportingDocumentId { get; set; }

        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        public string? FileName { get; set; }
        public string? FilePath { get; set; } // relative path or absolute
        public string? FileType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
