using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModerationService.Api.Models.Enums;

namespace ModerationService.Api.Models
{
    public class Sanction
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("report_id", TypeName = "char(36)")]
        public string? ReportId { get; set; }

        [Required]
        [Column("user_id", TypeName = "char(36)")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public SanctionType Type { get; set; }

        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        public string? Reason { get; set; }
    }
}