using caesar.data;
using caesar.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caesar.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CaesarDecryptController : ControllerBase
    {
        private readonly CaesarService _service;
        public CaesarDecryptController(CaesarService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var msg = await _service.GetByIdAsync(id);
            if (msg == null) return NotFound();
            var desencriptado = new {
                msg.Id,
                OriginalMessage = _service.Decrypt(msg.EncryptedMessage ?? "", msg.Shift),
                msg.EncryptedMessage,
                msg.Shift,
                msg.CreatedAt
            };
            return Ok(desencriptado);
        }
    }
}
