using System.Runtime.CompilerServices;

namespace LonerApp.Helpers
{
    public static class CheckLog
    {
        public static void WriteLine(string nameOfMethod, int number = 0, string data = "")
        {
            Console.WriteLine($" >>>>>>>>>> {nameOfMethod} {data} {number} ");
        }

        public static void Log(this object obj, object log, [CallerMemberName] string caller = null)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now.ToString("yyMMdd-hh:mm:ss.fff")}][{obj?.GetType().Name}][{caller}] {log?.ToString()}");
            }
            catch (Exception)
            {
            }
        }
    }
}