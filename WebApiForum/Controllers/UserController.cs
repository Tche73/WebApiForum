using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForum.Dto_s;
using WebApiForum.Dto_s.Details;
using WebApiForum.Models;

namespace WebApiForum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ForumDbContext _context;
        public UserController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDetailsDto>>> GetUsers()
        {
            return await _context.Users
                .Select(u => new UserDetailsDto
                {                  
                    Username = u.Username,
                    Email = u.Email
                })
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
            }

            return new UserDetailsDto
            {
                Username = user.Username,
                Email = user.Email
            };
        }

        //// GET: api/Users/5/messages
        //[HttpGet("{id}/messages")]
        //public async Task<ActionResult<IEnumerable<UserDetailsDto>>> GetUserMessages(int id)
        //{
        //    var userExists = await _context.Users.AnyAsync(u => u.Id == id);
        //    if (!userExists)
        //    {
        //        return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
        //    }

        //    return await _context.Messages
        //        .Where(m => m.UserId == id)
        //        .OrderByDescending(m => m.DatePublication)
        //        .Select(m => new UserDetailsDto
        //        {
        //            Id = m.Id,
        //            Contenu = m.Contenu,
        //            DatePublication = m.DatePublication,
        //            NombreReponses = m.Reponses.Count
        //        })
        //        .ToListAsync();
        //}

        //// GET: api/Users/5/reponses
        //[HttpGet("{id}/reponses")]
        //public async Task<ActionResult<IEnumerable<ReponseDto>>> GetUserReponses(int id)
        //{
        //    var userExists = await _context.Users.AnyAsync(u => u.Id == id);
        //    if (!userExists)
        //    {
        //        return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
        //    }

        //    return await _context.Reponses
        //        .Where(r => r.UserId == id)
        //        .OrderByDescending(r => r.DatePublication)
        //        .Select(r => new ReponseDto
        //        {
        //            Id = r.Id,
        //            Contenu = r.Contenu,
        //            DatePublication = r.DatePublication,
        //            MessageId = r.MessageId,
        //            MessageContenu = r.Message.Contenu,
        //            NombreLikes = r.Likes.Count
        //        })
        //        .ToListAsync();
        //}

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifier si l'email est déjà utilisé
            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
            {
                return Conflict($"L'email {dto.Email} est déjà utilisé.");
            }

            // Vérifier si le nom d'utilisateur est déjà utilisé
            var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
            if (usernameExists)
            {
                return Conflict($"Le nom d'utilisateur {dto.Username} est déjà utilisé.");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new User
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            });
        }

        //// PUT: api/Users/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(int id, UpdateUserDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
        //    }

        //    // Vérifier si le nouveau email est déjà utilisé par un autre utilisateur
        //    if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        //    {
        //        var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
        //        if (emailExists)
        //        {
        //            return Conflict($"L'email {dto.Email} est déjà utilisé.");
        //        }
        //        user.Email = dto.Email;
        //    }

        //    // Vérifier si le nouveau nom d'utilisateur est déjà utilisé par un autre utilisateur
        //    if (!string.IsNullOrEmpty(dto.Username) && dto.Username != user.Username)
        //    {
        //        var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id);
        //        if (usernameExists)
        //        {
        //            return Conflict($"Le nom d'utilisateur {dto.Username} est déjà utilisé.");
        //        }
        //        user.Username = dto.Username;
        //    }

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        throw;
        //    }

        //    return NoContent();
        //}

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"L'utilisateur avec l'ID {id} n'existe pas.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

