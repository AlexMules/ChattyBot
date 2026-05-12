namespace ChattyBot.Server.Application.BotEngine.Utils
{
    public static class RandomGenerator
    {
        public static int GetNext(int min, int max)
        {
            return Random.Shared.Next(min, max + 1);
        }
    }
}
