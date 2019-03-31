using System;

namespace Estranged.Workshop
{
    internal static class ConsoleHelpers
    {
        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void WriteLine(string line, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("  " + line);
            OverrideColors();
        }

        public static void Write(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            OverrideColors();
        }

        public static void FatalError(string error)
        {
            Console.WriteLine();
            WriteLine("FATAL ERROR", ConsoleColor.Red);
            Console.WriteLine();
            WriteLine(error, ConsoleColor.Red);
            Console.WriteLine();
            ConsoleHelpers.WriteLine($"If you keep seeing this error, you may want to report it on https://steamcommunity.com/app/{Constants.AppId}/discussions/ or https://discord.gg/estranged", ConsoleColor.Red);
            Program.PrimaryCancellationSource.Cancel();
        }

        public static void OverrideColors()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
