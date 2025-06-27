

namespace LeshopeChatBotUI
{
    public static class RuntimeLog
    {
        public static List<string> Logs { get; } = new();

        public static void Add(string log)
        {
            Logs.Add(log);
        }

        
    }
}
