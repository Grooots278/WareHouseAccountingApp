using System.ComponentModel.DataAnnotations;

namespace ServerPartProgram.DTOs.User_Dtos
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
