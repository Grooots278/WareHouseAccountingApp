using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerPartProgram.Models.User_Models
{
    //Для хранения истории паролей
    [Table("password_history")]
    public class PasswordHistory
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required]
        [Column("password_hash")]
        public string? PasswordHas { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
    }
}
