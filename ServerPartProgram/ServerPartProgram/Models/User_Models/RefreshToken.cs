using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerPartProgram.Models.User_Models
{
    //Модель для хранения refresh токенов
    [Table("refresh_tokens")]
    public class RefreshToken
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required]
        [Column("token")]
        public string? Token { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_by_ip")]
        [MaxLength(45)]
        public string? CreatedByIp { get; set; }

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        [Column("revoked_by_ip")]
        [MaxLength(45)]
        public string? RevokedByIp { get; set; }

        [Column("replaced_by_token")]
        public string? ReplcedByToken { get; set; }

        [Column("is_expired")]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        [Column("is_active")]
        public bool IsActive => RevokedAt == null && !IsExpired;

        public virtual User? User { get; set; }
    }
}
