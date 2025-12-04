using System.ComponentModel.DataAnnotations;

namespace ServerPartProgram.DTOs.User_Dtos
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Текущий пароль обязателен")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "Новый пароль обязателен")]
        [MinLength(8, ErrorMessage = "Новый пароль должен быть минимум 8 символов")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Подтверждение нового пароля обязательно")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
