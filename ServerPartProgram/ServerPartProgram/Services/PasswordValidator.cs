using ServerPartProgram.Models.User_Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ServerPartProgram.Services
{

    public interface IPasswordValidator
    {
        ValidationResult Validate(string password, User? user = null);
        bool IsPasswordInHistory(string passwordHash, ICollection<PasswordHistory> history, int historyCoung);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class PasswordValidator : IPasswordValidator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PasswordValidator> _logger;

        public PasswordValidator(IConfiguration configuration, ILogger<PasswordValidator> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public ValidationResult Validate(string password, User? user = null)
        {
            var result = new ValidationResult();
            var policy = _configuration.GetSection("PasswordPolicy");

            var minLength = policy.GetValue<int>("MinimumLength");

            if (password.Length < minLength)
                result.Errors.Add($"Пароль должен содержать минимум {minLength} символов");

            if (password.Length > 128)
                result.Errors.Add("Пароль не должен превышать 128 символов");

            if (policy.GetValue<bool>("RequireUpprecase") && !password.Any(char.IsUpper))
                result.Errors.Add("Пароль должен содержать хотя бы одну заглавную букву");

            if (policy.GetValue<bool>("RequireLowercase") && !password.Any(char.IsLower))
                result.Errors.Add("Пароль должен содержать хотя бы одну строчную букву");

            if (policy.GetValue<bool>("RequireDigit") && !password.Any(char.IsDigit))
                result.Errors.Add("Пароль должен содержать хотя бы одну цифру");

            if (policy.GetValue<bool>("RequireSpecialCharacter"))
            {
                var specialChars = policy.GetValue<string>("AllowedSpecialCharacters") ?? "!@#$%^&*()_+=-[]{}:;,.<>?";
                if (!password.Any(c => specialChars.Contains(c)))
                {
                    result.Errors.Add($"Пароль должен содержать хотя бы один специальный символ: {specialChars}");
                }
            }

            var maxConsecutive = policy.GetValue<int>("MaximumConsecutiveIdenticalChars");
            var consecutiveRegex = new Regex(@"(.)\1{" + (maxConsecutive - 1) + @",}");

            if(consecutiveRegex.IsMatch(password))
                result.Errors.Add($"Пароль не должен содержать более {maxConsecutive} одинаковых символов подряд");

            var commonPasswords = new[] { "password", "123456", "qwerty", "admin", "welcome" };
            if (commonPasswords.Contains(password.ToLower()))
                result.Errors.Add("Пароль слишком простой");

            if (user != null)
            {
                if (password.Contains(user.Username, StringComparison.OrdinalIgnoreCase) ||
                    password.Contains(user.Email.Split('@')[0], StringComparison.OrdinalIgnoreCase))
                    result.Errors.Add("Пароль не должен содержать имя пользователя или email");
            }

            if (IsSequential(password))
                result.Errors.Add("Пароль содержит последовательные символы");

            result.IsValid = result.Errors.Count == 0;

            if (!result.IsValid)
                _logger.LogWarning("Пароль не прошел валидацию: {Errors}", string.Join(", ", result.Errors));

            return result;
        }

        public bool IsPasswordInHistory(string passwordHash, ICollection<PasswordHistory> history, int historyCount)
        {
            if (history == null) return false;

            var recentHistory = history
                .OrderByDescending(h => h.CreatedAt)
                .Take(historyCount)
                .ToList();

            return recentHistory.Any(h => h.PasswordHas == passwordHash);
        }

        private bool IsSequential(string input)
        {
            if (input.Length < 3) return false;

            var sequences = new[]
            {
                "1234567890",
                "abcdefghijklmnopqrstuvwxyz",
                "qwertyuiopasdfghjklzxcvbnm",
                "йцукенгшщзхъфывапролджэячсмитьбю"
            };

            foreach (var sequence in sequences)
            {
                for (int i = 0; i <= sequence.Length - 3; i++)
                {
                    var seq = sequence.Substring(i, 3);
                    var revSeq = new string(seq.Reverse().ToArray());

                    if (input.ToLower().Contains(seq) || input.ToLower().Contains(revSeq))
                        return true;
                }
            }

            return false;
        }
    }
}
