using CommandLine;
using Estranged.Workshop.Options;
using Facepunch.Steamworks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Estranged.Workshop
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            const string banner = " Estranged: Act I Workshop Tool ";
            var separator = new string('=', banner.Length);

            ConsoleHelpers.WriteLine(separator, ConsoleColor.DarkGray);
            Console.WriteLine(banner);
            ConsoleHelpers.WriteLine(separator, ConsoleColor.DarkGray);
            Console.WriteLine();

            var arguments = Parser.Default.ParseArguments<MountOptions, UploadOptions>(args);

            var cancellationSource = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, ev) =>
            {
                ev.Cancel = true;
                cancellationSource.Cancel();
            };

            var services = new ServiceCollection()
                .AddSingleton<WorkshopMounter>()
                .AddSingleton<WorkshopRepository>()
                .AddSingleton<WorkshopUploader>()
                .AddSingleton<GameInfoRepository>()
                .AddSingleton<BrowserOpener>();

            using (var steam = new Client(Constants.AppId))
            using (Task.Run(() => TickSteamClient(steam, cancellationSource.Token)))
            using (var provider = services.AddSingleton(steam).BuildServiceProvider())
            {
                // Add newlines after the Steam SDK spam
                Console.WriteLine();

                arguments.MapResult((MountOptions options) => Mount(provider, options, cancellationSource.Token),
                                    (UploadOptions options) => Upload(provider, options, cancellationSource.Token),
                                    errors => 1);

                cancellationSource.Cancel();
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

            return 0;
        }

        private static int Upload(IServiceProvider provider, UploadOptions options, CancellationToken token)
        {
            provider.GetRequiredService<WorkshopUploader>()
                    .Upload(options.UploadDirectory, token)
                    .GetAwaiter()
                    .GetResult();

            return 0;
        }
    }
}
