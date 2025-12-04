using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerPartProgram.Models.User_Models
{
    [Table("audit_logs")]
    public class AuditLog
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("user_id")]
        public Guid? UserId { get; set; }

        [Required]
        [Column("action")]
        [MaxLength(100)]
        public string? Action { get; set; } // "REGISTER", "LOGIN", "LOGOUT", "CHANGE_PASSWORD"

        [Column("details")]
        public string? Details { get; set; }

        [Column("ip_address")]
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        public string? UserAgent { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
