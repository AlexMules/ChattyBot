namespace ChattyBot.Server.Application.BotEngine.Utils
{
    public static class CaesarCipher
    {
        public static string Encrypt(string input, int shift = 3)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            char[] buffer = input.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                char letter = buffer[i];

                if (char.IsLetter(letter))
                {
                    char offset = char.IsUpper(letter) ? 'A' : 'a';
                    buffer[i] = (char)(((letter + shift - offset) % 26) + offset);
                }
            }
            return new string(buffer);
        }
    }
}
