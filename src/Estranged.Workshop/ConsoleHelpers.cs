using System;

namespace Estranged.Workshop
{
    internal static class ConsoleHelpers
    {
        public static void WriteLine(string line, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void FatalError(string error)
        {
            Console.WriteLine();
            WriteLine("FATAL ERROR", ConsoleColor.Red);
            Console.WriteLine();
            WriteLine(error, ConsoleColor.Red);
            Console.WriteLine();
            ConsoleHelpers.WriteLine($"If you keep seeing this error, you may want to report it on https://steamcommunity.com/app/{Constants.AppId}/discussions/ or https://discord.gg/estranged", ConsoleColor.Red);
            Environment.Exit(1);
        }
    }
}
