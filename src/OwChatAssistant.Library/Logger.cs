namespace OwChatAssistant.Library
{
    public static class Logger
    {
        public static void Log(string message)
        {
            var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Console.WriteLine(log) ;
            System.Diagnostics.Debug.WriteLine(log);
        }
    }
}
