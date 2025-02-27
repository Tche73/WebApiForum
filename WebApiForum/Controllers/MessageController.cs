using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForum.Dto_s;
using WebApiForum.Models;

namespace WebApiForum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ForumDbContext _context;

        public MessagesController(ForumDbContext context)
        {
            _context = context;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            // Chargez les messages avec les relations nécessaires
            var messages = await _context.Messages
                .Include(m => m.User) // Ajoutez les includes nécessaires
                .Include(m => m.Reponses)
                    .ThenInclude(r => r.User)
                    .OrderByDescending(m => m.DatePublication)   
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages
                .Include(m => m.Reponses)
                    .ThenInclude(r => r.Likes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound($"Le message avec l'ID {id} n'existe pas.");
            }

            return message;
        }

        // POST: api/Messages
        // POST: api/Messages
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(CreateMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez que l'utilisateur existe
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return BadRequest($"L'utilisateur avec l'ID {dto.UserId} n'existe pas.");
            }

            var message = new Message
            {
                Contenu = dto.Contenu,
                DatePublication = DateTime.Now,
                UserId = dto.UserId  
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, UpdateMessageDto dto)
        {
          

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingMessage = await _context.Messages.FindAsync(id);
                if (existingMessage == null)
                {
                    return NotFound($"Le message avec l'ID {id} n'existe pas.");
                }

                // Mise à jour uniquement des propriétés modifiables
                existingMessage.Contenu = dto.Contenu;
                existingMessage.DatePublication = DateTime.Now;
                // Vous pouvez ajouter d'autres propriétés modifiables ici

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound($"Le message avec l'ID {id} n'existe pas.");
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Messages/5/reponses
        [HttpGet("{id}/Reponses")]
        public async Task<ActionResult<IEnumerable<Reponse>>> GetMessageReponses(int id)
        {
            if (!MessageExists(id))
            {
                return NotFound($"Le message avec l'ID {id} n'existe pas.");
            }

            var reponses =  await _context.Reponses
                .Where(r => r.MessageId == id)
                .Include(r => r.User)
                .Include(r => r.Likes)
                .OrderBy(r => r.DatePublication)
                .ToListAsync();

            return reponses;
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}