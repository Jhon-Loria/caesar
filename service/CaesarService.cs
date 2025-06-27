using System;

namespace caesar.service
{
    public class CaesarService
    {
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
