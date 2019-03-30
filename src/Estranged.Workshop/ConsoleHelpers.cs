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
    }
}
