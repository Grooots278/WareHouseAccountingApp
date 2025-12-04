namespace ServerPartProgram.DTOs.User_Dtos
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? AccessTokenExpiration { get; set; }
        public DateTime? RefreshTokenExpiration {  get; set; }
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}
