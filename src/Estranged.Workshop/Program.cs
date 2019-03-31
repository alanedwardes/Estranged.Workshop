using CommandLine;
using Estranged.Workshop.Options;
using Facepunch.Steamworks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Estranged.Workshop
{
    internal sealed class Program
    {
        public static CancellationTokenSource PrimaryCancellationSource = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            const string banner = " Estranged: Act I Workshop Tool ";
            var separator = new string('=', banner.Length);

            ConsoleHelpers.WriteLine(separator, ConsoleColor.DarkGray);
            Console.WriteLine(banner);
            ConsoleHelpers.WriteLine(separator, ConsoleColor.DarkGray);
            Console.WriteLine();

            var arguments = Parser.Default.ParseArguments<MountOptions, UploadOptions>(args);

            Console.CancelKeyPress += (sender, ev) =>
            {
                ev.Cancel = true;
                PrimaryCancellationSource.Cancel();

                Console.WriteLine();
                ConsoleHelpers.WriteLine("Cancelling...", ConsoleColor.Yellow);
            };

            var services = new ServiceCollection()
                .AddSingleton<WorkshopMounter>()
                .AddSingleton<WorkshopRepository>()
                .AddSingleton<WorkshopUploader>()
                .AddSingleton<GameInfoRepository>()
                .AddSingleton<BrowserOpener>();

            using (var steam = new Client(Constants.AppId))
            using (var tick = Task.Run(() => TickSteamClient(steam, PrimaryCancellationSource.Token)))
            using (var provider = services.AddSingleton(steam).BuildServiceProvider())
            {
                // Add newlines after the Steam SDK spam
                Console.WriteLine();

                if (steam.IsValid)
                {
                    try
                    {
                        arguments.MapResult((MountOptions options) => Mount(provider, options, PrimaryCancellationSource.Token),
                        (UploadOptions options) => Upload(provider, options, PrimaryCancellationSource.Token),
                        errors => 1);
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected
                    }
                }
                else
                {
                    ConsoleHelpers.FatalError("Failed to connect to Steam.");
                }

                PrimaryCancellationSource.Cancel();

                try
                {
                    tick.GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    // Expected
                }
            }
        }

        private static async Task TickSteamClient(Client steam, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                steam.Update();
                await Task.Delay(TimeSpan.FromMilliseconds(20), token);
            }
        }

        private static int Mount(IServiceProvider provider, MountOptions options, CancellationToken token)
        {
            provider.GetRequiredService<WorkshopMounter>()
                    .Mount(token)
                    .GetAwaiter()
                    .GetResult();

            Console.WriteLine();
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
            return 0;
        }

        private static int Upload(IServiceProvider provider, UploadOptions options, CancellationToken token)
        {
            if (options.Interactive)
            {
                Console.WriteLine("Enter the numeric file ID or workshop item URL to update, followed by <enter>: ");
                Console.WriteLine();

                var input = Console.ReadLine().Trim();
                if (ulong.TryParse(input, out var rawFileId))
                {
                    options.ExistingItem = rawFileId;
                }
                else if (ulong.TryParse(Regex.Match(input, @"\?id=(?<fileId>[0-9]*)").Groups["fileId"].Value, out var urlFileId))
                {
                    options.ExistingItem = urlFileId;
                }
                else
                {
                    ConsoleHelpers.FatalError($"Unable to find workshop item ID in '{input}'");
                }
            }

            provider.GetRequiredService<WorkshopUploader>()
                    .Upload(options.UploadDirectory, options.ExistingItem, token)
                    .GetAwaiter()
                    .GetResult();

            return 0;
        }
    }
}
