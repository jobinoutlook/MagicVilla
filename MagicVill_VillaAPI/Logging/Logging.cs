
namespace MagicVilla_VillaAPI
{
    public class Logging : ILogging
    {
        public void Log(string message, LogType type)
        {
            if (type == LogType.ERROR)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR - " + message);
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else if (type == LogType.WARNING)
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine("WARNING - " + message);
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else if (type == LogType.INFO)
            {
                Console.WriteLine("INFO - " + message);
            }
        }
    }
}
