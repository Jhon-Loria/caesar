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
        private readonly CaesarMessageRepository _repo;
        private readonly CaesarService _service;
        public CaesarDecryptController(CaesarMessageRepository repo, CaesarService service)
        {
            _repo = repo;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var messages = await _repo.GetAllAsync();
            var desencriptados = messages.Select(m => new {
                m.Id,
                DecryptedMessage = _service.Decrypt(m.EncryptedMessage ?? "", m.Shift),
                m.EncryptedMessage,
                m.Shift,
                m.CreatedAt
            });
            return Ok(desencriptados);
        }
    }
}
