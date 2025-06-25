using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.PDV.Domain;
using API.PDV.Infra;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace API.PDV.WebApi
{
    /// <summary>
    /// Controller para autenticação de usuários e geração de JWT.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly string _jwtKey;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            // Busca chave JWT do appsettings ou usa padrão forte
            _jwtKey = config["Jwt:Key"] ?? "SENHA_FORTE_PADRAO_1234567890_ABCDEFGH";
        }

        /// <summary>
        /// Realiza login e retorna um token JWT para autenticação.
        /// </summary>
        /// <param name="login">Credenciais do usuário.</param>
        /// <returns>Token JWT válido para autenticação Bearer.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Busca case-insensitive
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == login.Username.ToLower());

            if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
                return Unauthorized("Usuário ou senha inválidos.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("TenantSlug", user.TenantSlug),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "API.PDV",
                audience: "API.PDV.Client",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="register">Dados do novo usuário.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Normaliza username para minúsculo
            var normalizedUsername = register.Username.ToLowerInvariant();

            if (await _db.Users.AnyAsync(u => u.Username.ToLower() == normalizedUsername))
                return BadRequest("Nome de usuário já cadastrado.");

            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == register.Email.ToLower()))
                return BadRequest("E-mail já cadastrado.");

            var user = new User
            {
                Username = normalizedUsername,
                Email = register.Email,
                PasswordHash = HashPassword(register.Password),
                TenantSlug = register.TenantSlug,
                Role = register.Role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Usuário registrado com sucesso.",
                usuario = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.TenantSlug,
                    user.Role
                }
            });
        }

        /// <summary>
        /// Gera hash seguro para senha.
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifica se a senha corresponde ao hash.
        /// </summary>
        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }

    /// <summary>
    /// DTO para login.
    /// </summary>
    public class LoginDto
    {
        /// <example>admin</example>
        [Required]
        public string Username { get; set; } = string.Empty;
        /// <example>admin123</example>
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para registro de usuário.
    /// </summary>
    public class RegisterDto
    {
        /// <example>admin</example>
        [Required]
        public string Username { get; set; } = string.Empty;
        /// <example>admin@empresa.com</example>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        /// <example>SenhaForte123</example>
        [Required]
        public string Password { get; set; } = string.Empty;
        /// <example>empresa-exemplo</example>
        [Required]
        public string TenantSlug { get; set; } = string.Empty;
        /// <example>Admin</example>
        [Required]
        public string Role { get; set; } = "Admin";
    }
}