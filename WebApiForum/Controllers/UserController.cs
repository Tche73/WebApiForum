using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForum.Dto_s;
using WebApiForum.Dto_s.Details;
using WebApiForum.Dto_s.Update;
using WebApiForum.Models;

namespace WebApiForum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ForumDbContext _context;
        public UserController(UserManager<User> userManager, ForumDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDetailsDto>>> GetUsers()
        {
            var users = await _userManager.Users
                .Select(u => new UserDetailsDto
                {
                    Username = u.UserName,
                    Email = u.Email
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsDto>> GetUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
            }

            return new UserDetailsDto
            {
                Username = user.UserName,
                Email = user.Email
            };
        }



        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifier si l'email est déjà utilisé
            var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
            {
                return Conflict($"L'email {dto.Email} est déjà utilisé.");
            }

            // Vérifier si le nom d'utilisateur est déjà utilisé
            var existingUserByUsername = await _userManager.FindByNameAsync(dto.Username);
            if (existingUserByUsername != null)
            {
                return Conflict($"Le nom d'utilisateur {dto.Username} est déjà utilisé.");
            }

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            // Utiliser CreateAsync pour gérer la création sécurisée de l'utilisateur
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                var userDetails = new UserDetailsDto
                {
                    Username = user.UserName,
                    Email = user.Email
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDetails);
            }

            // Gérer les erreurs de création
            return BadRequest(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
                }

                // Mise à jour uniquement des propriétés modifiables
                existingUser.UserName = dto.UserName;
                existingUser.Displayname = dto.DisplayName;
                // Vous pouvez ajouter d'autres propriétés modifiables ici

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);

        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

