using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Estranged.Workshop
{
    internal sealed class WorkshopMounter
    {
        private readonly WorkshopRepository _workshopRepository;
        private readonly GameInfoRepository _gameInfoRepository;

        public WorkshopMounter(WorkshopRepository repository, GameInfoRepository gameInfoRepository)
        {
            _workshopRepository = repository;
            _gameInfoRepository = gameInfoRepository;
        }

        public async Task Mount(CancellationToken token)
        {
            var gameInfo = _gameInfoRepository.FindGameInfo();

            var items = await _workshopRepository.GetUserSubscribedItems(token);
            if (items.Length == 0)
            {
                ConsoleHelpers.WriteLine($"No workshop items found. To subscribe to items, head over to the workshop!", ConsoleColor.Yellow);
                ConsoleHelpers.WriteLine();
                ConsoleHelpers.WriteLine($"https://steamcommunity.com/app/{Constants.AppId}/workshop/", ConsoleColor.Blue);
                return;
            }

            var itemsToMount = new List<Item>();

            ConsoleHelpers.WriteLine($"Found {items.Length} workshop item(s) in total:", ConsoleColor.Yellow);
            ConsoleHelpers.WriteLine();
            foreach (var item in items)
            {
                ConsoleHelpers.Write("\t- ", ConsoleColor.DarkGray);
                ConsoleHelpers.Write(item.Title, ConsoleColor.White);
                if (item.IsInstalled)
                {
                    ConsoleHelpers.Write(" (will be mounted)", ConsoleColor.Gray);
                    itemsToMount.Add(item);
                }
                else
                {
                    ConsoleHelpers.Write(" (won't be mounted - not installed)", ConsoleColor.DarkGray);
                }
                ConsoleHelpers.WriteLine();
            }

            var itemDirectories = items.Where(x => x.IsInstalled).Select(x => new DirectoryInfo(x.Directory)).ToArray();

            await _gameInfoRepository.SetWorkshopSearchPaths(gameInfo, itemDirectories);

            ConsoleHelpers.WriteLine();

            if (itemsToMount.Count > 0)
            {
                ConsoleHelpers.WriteLine($"You can now run the game and play your new content!", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelpers.WriteLine($"No items found to mount. Looks like they are still installing.", ConsoleColor.Yellow);
            }
        }
    }
}
