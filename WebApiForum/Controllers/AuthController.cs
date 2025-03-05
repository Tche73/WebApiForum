using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiForum.Dto_s;
using WebApiForum.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        // Log détaillé des données reçues
        Console.WriteLine($"Username reçu : {model.Username}");
        Console.WriteLine($"Email reçu : {model.Email}");
        Console.WriteLine($"Password reçu : {model.Password != null}");
        try
        {
            // Validation explicite du modèle
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez si l'email existe déjà
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                return BadRequest(new { message = "Un utilisateur avec cet email existe déjà" });
            }

            // Vérifiez si le nom d'utilisateur existe déjà
            var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
            if (existingUserByUsername != null)
            {
                return BadRequest(new { message = "Un utilisateur avec ce nom d'utilisateur existe déjà" });
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Displayname = model.Username
                // Ajoutez d'autres propriétés si nécessaire
            };

            // Tentative de création de l'utilisateur
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "Utilisateur créé avec succès" });
            }

            // Si la création échoue, retournez les erreurs
            return BadRequest(new
            {
                message = "Échec de la création de l'utilisateur",
                errors = result.Errors.Select(e => e.Description).ToList()
            });
        }
        catch (Exception ex)
        {
            // Logging détaillé
            Console.WriteLine($"Erreur complète : {ex}");
            Console.WriteLine($"Message de l'exception : {ex.Message}");

            // Si une exception interne existe, loguez-la
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Exception interne : {ex.InnerException}");
                Console.WriteLine($"Message de l'exception interne : {ex.InnerException.Message}");
            }

            return StatusCode(500, new
            {
                message = "Une erreur est survenue lors de l'inscription",
                detailedError = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    //[HttpPost("register")]
    //public async Task<IActionResult> Register([FromBody] RegisterDto model)
    //{

    //    var user = new User
    //    {
    //        UserName = model.Username,
    //        Email = model.Email
    //    };

    //    var result = await _userManager.CreateAsync(user, model.Password);

    //    if (result.Succeeded)
    //    {
    //        return Ok(new { Message = "Utilisateur enregistré avec succès" });
    //    }

    //    return BadRequest(result.Errors);
    //}

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Utilisateur non trouvé");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (result.Succeeded)
        {
            var token = GenerateJwtToken(user);
            var expirationDate = DateTime.UtcNow.AddDays(7); // Correspond à l'expiration du token

            var authResponse = new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Token = token,
                ExpirationDate = expirationDate
            };

            return Ok(authResponse);
        }

        return Unauthorized("Mot de passe incorrect");
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}