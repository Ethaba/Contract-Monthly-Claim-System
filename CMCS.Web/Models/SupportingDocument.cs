using System.ComponentModel.DataAnnotations;

namespace CMCS.Web.Models
{
    public class SupportingDocument
    {
        [Key]
        public int SupportingDocumentId { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileType { get; set; } = null!;
    }
}
