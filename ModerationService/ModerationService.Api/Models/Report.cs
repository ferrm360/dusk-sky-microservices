using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModerationService.Api.Models.Enums;

namespace ModerationService.Api.Models
{
    public class Report
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("reported_user_id", TypeName = "char(36)")]
        public string ReportedUserId { get; set; } = string.Empty;

        [Required]
        public ContentType ContentType { get; set; }

        public string? Reason { get; set; }

        [Column("reported_at")]
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        public ReportStatus Status { get; set; } = ReportStatus.pending;
    }
}