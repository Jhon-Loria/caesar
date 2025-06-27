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
        private readonly CaesarService _service;
        public CaesarEncryptController(CaesarService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var messages = await _service.GetAllAsync();
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
            var msg = await _service.GetByIdAsync(id);
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
            var result = await _service.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CaesarMessage model)
        {
            await _service.UpdateAsync(id, model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
