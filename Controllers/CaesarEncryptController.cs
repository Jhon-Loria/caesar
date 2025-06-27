using caesar.data;
using caesar.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caesar.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CaesarEncryptController : ControllerBase
    {
        private readonly CaesarMessageRepository _repo;
        private readonly CaesarService _service;
        public CaesarEncryptController(CaesarMessageRepository repo, CaesarService service)
        {
            _repo = repo;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var messages = await _repo.GetAllAsync();
            var encriptados = messages.Select(m => new {
                m.Id,
                EncryptedMessage = m.EncryptedMessage,
                m.Shift,
                m.CreatedAt
            });
            return Ok(encriptados);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var msg = await _repo.GetByIdAsync(id);
            if (msg == null) return NotFound();
            var encriptado = new {
                msg.Id,
                EncryptedMessage = msg.EncryptedMessage,
                msg.Shift,
                msg.CreatedAt
            };
            return Ok(encriptado);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CaesarMessage model)
        {
            // Si el shift no se proporciona o es 0, genera uno aleatorio entre 1 y 25
            if (model.Shift == 0)
            {
                var random = new Random();
                model.Shift = random.Next(1, 26); // 1 a 25
            }
            model.EncryptedMessage = _service.Encrypt(model.OriginalMessage ?? "", model.Shift);
            await _repo.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CaesarMessage model)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.OriginalMessage = model.OriginalMessage;
            existing.Shift = model.Shift;
            existing.EncryptedMessage = _service.Encrypt(model.OriginalMessage ?? "", model.Shift);
            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
