using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerPartProgram.Models.User_Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Email обязательно")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Column("email")]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [Column("username")]
        [MinLength(3, ErrorMessage = "Имя пользователя должно быть минимум 3 символа")]
        [MaxLength(50, ErrorMessage = "Имя пользователя не должно превышать 50 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Имя пользователя может содержать только буквы, цифры, точку, дефис и подчеркивание")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("last_login_at")]
        public DateTime? LastUpdatedAt { get; set; }

        [Column("failed_login_attempts")]
        public int FailedLoginAttempts { get; set; } = 0;

        [Column("is_locked")]
        public bool IsLocked { get; set; } = false;

        [Column("locked_until")]
        public DateTime? LockedUntil { get; set; }

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }

        public virtual ICollection<PasswordHistory>? PasswordHistories { get; set; }
    }
}
