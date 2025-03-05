using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiForum.Dto_s;
using WebApiForum.Models;

namespace WebApiForum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReponsesController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public ReponsesController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/Reponses
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReponseDetailsDto>>> GetReponses()
        {
                var reponses = await _context.Reponses
             .Include(r => r.Message)
             .Include(r => r.Likes)
             .OrderByDescending(r => r.DatePublication)
             .Select(r => new ReponseDetailsDto
             {                
                 MessageParent = r.Message.Contenu,
                 Contenu = r.Contenu,
                 DatePublication = r.DatePublication,
                 NombreLikes = r.Likes.Count
             })
             .ToListAsync();
            return reponses;
        }

        // GET: api/Reponses/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ReponseDetailsDto>> GetReponse(int id)
        {
            var reponse = await _context.Reponses
                .Include(r => r.Likes)
                .Include(r => r.Message)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reponse == null)
            {
                return NotFound($"La réponse avec l'ID {id} n'existe pas.");
            }
            var reponseDto = new ReponseDetailsDto
            {
                MessageParent = reponse.Message.Contenu,
                Contenu = reponse.Contenu,
                DatePublication = reponse.DatePublication,
                NombreLikes = reponse.Likes.Count
            };

            return reponseDto;
        }

        // POST: api/Reponses
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Reponse>> PostReponse(CreateReponseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // Vérifier si le message parent existe
            var messageExists = await _context.Messages.AnyAsync(m => m.Id == dto.MessageId);
            if (!messageExists)
            {
                return BadRequest($"Le message parent avec l'ID {dto.MessageId} n'existe pas.");
            }

            // Vérifier si l'utilisateur existe
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return BadRequest($"L'utilisateur avec l'ID {dto.UserId} n'existe pas.");
            }

            Reponse reponse = new Reponse
            {
                Contenu = dto.Contenu,
                DatePublication = DateTime.Now,
                MessageId = dto.MessageId,
                UserId = dto.UserId,
            };

            _context.Reponses.Add(reponse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReponse), new { id = reponse.Id }, reponse);
        }

        // PUT: api/Reponses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReponse(int id, UpdateReponseDto dto)
        {       
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingReponse = await _context.Reponses.FindAsync(id);
                if (existingReponse == null)
                {
                    return NotFound($"La réponse avec l'ID {id} n'existe pas.");
                }

                existingReponse.Contenu = dto.Contenu;
                existingReponse.DatePublication = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await ReponseExists(id)))  // Correction ici
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Reponses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReponse(int id)
        {
            var reponse = await _context.Reponses.FindAsync(id);
            if (reponse == null)
            {
                return NotFound($"La réponse avec l'ID {id} n'existe pas.");
            }

            _context.Reponses.Remove(reponse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //// GET: api/Reponses/message/5
        //[HttpGet("message/{messageId}")]
        //public async Task<ActionResult<IEnumerable<Reponse>>> GetReponsesByMessage(int messageId)
        //{
        //    // Vérifier si le message existe
        //    var messageExists = await _context.Messages.AnyAsync(m => m.Id == messageId);
        //    if (!messageExists)
        //    {
        //        return NotFound($"Le message avec l'ID {messageId} n'existe pas.");
        //    }

        //    return await _context.Reponses
        //        .Where(r => r.MessageId == messageId)
        //        .Include(r => r.Likes)
        //        .OrderBy(r => r.DatePublication)
        //        .ToListAsync();
        //}
        // GET: api/Reponses/5/Likes

        [HttpGet("{id}/Likes")]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikesForReponse(int id)
        {
            var reponse = await _context.Reponses.FindAsync(id);
            if (reponse == null)
            {
                return NotFound();
            }

            var likes = await _context.Likes
                .Where(l => l.ReponseId == id)
                .Include(l => l.User)
                .ToListAsync();

            return likes;
        }

        private async Task<bool> ReponseExists(int id)
        {
            return await _context.Reponses.AnyAsync(e => e.Id == id);
        }
    }
}