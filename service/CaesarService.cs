using caesar.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace caesar.service
{
    public class CaesarService
    {
        private readonly CaesarMessageRepository _repo;
        public CaesarService(CaesarMessageRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CaesarMessage>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<CaesarMessage?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<CaesarMessage> AddAsync(CaesarMessage model)
        {
            if (model.Shift == 0)
            {
                var random = new Random();
                model.Shift = random.Next(1, 26);
            }
            model.EncryptedMessage = Encrypt(model.OriginalMessage ?? "", model.Shift);
            await _repo.AddAsync(model);
            return model;
        }

        public async Task UpdateAsync(int id, CaesarMessage model)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return;
            existing.OriginalMessage = model.OriginalMessage;
            existing.Shift = model.Shift;
            existing.EncryptedMessage = Encrypt(model.OriginalMessage ?? "", model.Shift);
            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public string Encrypt(string message, int shift)
        {
            return new string(message.Select(c => ShiftChar(c, shift)).ToArray());
        }

        public string Decrypt(string encryptedMessage, int shift)
        {
            return new string(encryptedMessage.Select(c => ShiftChar(c, -shift)).ToArray());
        }

        private char ShiftChar(char c, int shift)
        {
            if (!char.IsLetter(c)) return c;
            char offset = char.IsUpper(c) ? 'A' : 'a';
            return (char)(((c + shift - offset + 26) % 26) + offset);
        }
    }
}
