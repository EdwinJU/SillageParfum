using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SillageParfumApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // UserManager es la herramienta de Identity que guarda y valida contraseñas encriptadas
        private readonly UserManager<IdentityUser> _userManager;
        // IConfiguration nos permite leer tu appsettings.json para sacar la Llave Secreta
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // POST: api/auth/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UserDto modelo)
        {
            // 1. Preparamos al usuario
            var usuario = new IdentityUser { UserName = modelo.Email, Email = modelo.Email };

            // 2. Le pedimos a Identity que lo guarde en la BD y encripte su contraseña
            var resultado = await _userManager.CreateAsync(usuario, modelo.Password);

            if (resultado.Succeeded)
            {
                return Ok(new { mensaje = "¡Usuario creado con éxito!" });
            }

            // Si falla (ej. la contraseña es muy débil), devolvemos los errores
            return BadRequest(resultado.Errors);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto modelo)
        {
            // 1. Buscamos al usuario por correo
            var usuario = await _userManager.FindByEmailAsync(modelo.Email);

            // 2. Verificamos que exista y que la contraseña coincida
            if (usuario != null && await _userManager.CheckPasswordAsync(usuario, modelo.Password))
            {
                // 3. ¡Todo correcto! Vamos a fabricarle su Gafete (Token JWT)
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // Traemos la llave secreta desde el appsettings.json
                var llaveSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3), // El token durará 3 horas
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(llaveSecreta, SecurityAlgorithms.HmacSha256)
                );

                // Devolvemos el token al frontend
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiracion = token.ValidTo
                });
            }

            return Unauthorized(new { mensaje = "Contraseña o correo incorrectos." });
        }
    }

    // Esta es la "cajita" (DTO - Data Transfer Object) para recibir los datos de Swagger/React
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}