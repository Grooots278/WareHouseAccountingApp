using Microsoft.IdentityModel.Tokens;
using ServerPartProgram.Models.User_Models;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServerPartProgram.Services
{

    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        DateTime GetAccessTokenExpiration();
        DateTime GetRefreshTokenExpiration();
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;
        private readonly SymmetricSecurityKey _signingKey;

        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 64)
                throw new ArgumentException("JWT secret key должен быть минимум 64 символа");

            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string GenerateAccessToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("username", user.Username),
                        new Claim("created_at", user.CreatedAt.ToString("O"))
                    }),
                    Expires = GetAccessTokenExpiration(),
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(
                        _signingKey,
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации access токена");
                throw;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public DateTime GetAccessTokenExpiration()
        {
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes", 15);
            return DateTime.UtcNow.AddMinutes(expirationMinutes);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = false,
                ClockSkew = TimeSpan.FromMinutes(
                    _configuration.GetValue<int>("JwtSettings:ClockSkewMinutes", 1))
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при валидации просроченного токена");
                throw;
            }
        }

        public DateTime GetRefreshTokenExpiration()
        {
            var expirationDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays", 7);
            return DateTime.UtcNow.AddDays(expirationDays);
        }
    }
}
