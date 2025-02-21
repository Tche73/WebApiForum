using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForum.Models;

namespace WebApiForum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public LikeController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/Likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
        {
            return await _context.Likes.ToListAsync();
        }

        // GET: api/Likes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Like>> GetLike(int id)
        {
            var like = await _context.Likes.FindAsync(id);

            if (like == null)
            {
                return NotFound();
            }

            return like;
        }

        // GET: api/Likes/reponse/5
        [HttpGet("reponse/{reponseId}")]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikesByReponse(int reponseId)
        {
            // Vérifier si la réponse existe
            var reponseExists = await _context.Reponses.AnyAsync(r => r.Id == reponseId);
            if (!reponseExists)
            {
                return NotFound($"La réponse avec l'ID {reponseId} n'existe pas.");
            }

            return await _context.Likes
                .Where(l => l.ReponseId == reponseId)
                .ToListAsync();
        }

        // POST: api/Likes
        [HttpPost]
        public async Task<ActionResult<Like>> PostLike(Like like)
        {
            // Vérifier si la réponse existe
            var reponseExists = await _context.Reponses.AnyAsync(r => r.Id == like.ReponseId);
            if (!reponseExists)
            {
                return BadRequest($"La réponse avec l'ID {like.ReponseId} n'existe pas.");
            }

            // Vérifier si l'utilisateur a déjà liké cette réponse
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.ReponseId == like.ReponseId && l.UserId == like.UserId);

            if (existingLike != null)
            {
                return Conflict("Vous avez déjà liké cette réponse.");
            }

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLike), new { id = like.Id }, like);
        }

        // DELETE: api/Likes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLike(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Likes/user/{userId}/reponse/{reponseId}
        [HttpDelete("user/{userId}/reponse/{reponseId}")]
        public async Task<IActionResult> DeleteLikeByUserAndReponse(string userId, int reponseId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ReponseId == reponseId);

            if (like == null)
            {
                return NotFound("Aucun like trouvé pour cet utilisateur sur cette réponse.");
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Likes/count/reponse/5
        [HttpGet("count/reponse/{reponseId}")]
        public async Task<ActionResult<int>> GetLikesCountForReponse(int reponseId)
        {
            // Vérifier si la réponse existe
            var reponseExists = await _context.Reponses.AnyAsync(r => r.Id == reponseId);
            if (!reponseExists)
            {
                return NotFound($"La réponse avec l'ID {reponseId} n'existe pas.");
            }

            return await _context.Likes.CountAsync(l => l.ReponseId == reponseId);
        }
    }
}