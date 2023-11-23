using System;
using System.Threading;
using System.Threading.Tasks;

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
            WriteLine($"If you keep seeing this error, you may want to report it:", ConsoleColor.Yellow);
            WriteLine($"- https://steamcommunity.com/app/{Constants.AppId}/discussions/", ConsoleColor.Yellow);
            WriteLine($"- https://discord.gg/estranged", ConsoleColor.Yellow);

            Program.PrimaryCancellationSource.Cancel();

            WaitBeforeExiting(TimeSpan.FromSeconds(5), CancellationToken.None).GetAwaiter().GetResult();
        }

        public static void OverrideColors()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static async Task WaitBeforeExiting(TimeSpan waitTime, CancellationToken token)
        {
            Console.WriteLine();

            var pauseTime = TimeSpan.FromSeconds(1);

            while (waitTime > TimeSpan.Zero)
            {
                Console.WriteLine($"Exiting in {waitTime.TotalSeconds} seconds...");
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                await Task.Delay(pauseTime, token);
                waitTime -= pauseTime;
            }
        }
    }
}