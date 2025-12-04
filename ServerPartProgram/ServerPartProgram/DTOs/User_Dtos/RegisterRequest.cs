using System.ComponentModel.DataAnnotations;

namespace ServerPartProgram.DTOs.User_Dtos
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [MaxLength(254, ErrorMessage = "Email не должен превышать 254 символа")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [MinLength(3, ErrorMessage = "Имя пользователя должно быть минимум 3 символа")]
        [MaxLength(50, ErrorMessage = "Имя пользователя не должно превышать 50 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Имя пользователя может содержать только буквы, цифры, точку, дефис и подчеркивание")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен быть минимум 8 символов")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

    }
}
