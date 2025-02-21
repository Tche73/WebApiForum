﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return await _context.Messages
                .Include(m => m.Reponses)
                .OrderByDescending(m => m.DatePublication)
                .ToListAsync();
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
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest("L'ID du message ne correspond pas.");
            }

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
                existingMessage.Contenu = message.Contenu;
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
        [HttpGet("{id}/reponses")]
        public async Task<ActionResult<IEnumerable<Reponse>>> GetMessageReponses(int id)
        {
            if (!MessageExists(id))
            {
                return NotFound($"Le message avec l'ID {id} n'existe pas.");
            }

            return await _context.Reponses
                .Where(r => r.MessageId == id)
                .Include(r => r.Likes)
                .OrderBy(r => r.DatePublication)
                .ToListAsync();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}