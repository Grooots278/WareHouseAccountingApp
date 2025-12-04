using System.ComponentModel.DataAnnotations;

namespace ServerPartProgram.DTOs.User_Dtos
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Access токен обязателен")]
        public string? AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh токен обязателен")]
        public string? RefreshToken { get; set; }
    }
}
