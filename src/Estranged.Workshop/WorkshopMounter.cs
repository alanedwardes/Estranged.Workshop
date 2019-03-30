using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Facepunch.Steamworks.Workshop;

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

            var itemsToMount = new List<Item>();

            Console.WriteLine($"Found {items.Length} workshop item(s) in total:");
            foreach (var item in items)
            {
                Console.Write($"* {item.Title}");
                if (item.Installed)
                {
                    Console.Write(" (will be mounted)");
                    itemsToMount.Add(item);
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine($"Mounting {string.Join(" ,", itemsToMount.Select(x => x.Title))}");

            var itemDirectories = items.Where(x => x.Installed).Select(x => x.Directory).ToArray();

            await _gameInfoRepository.SetWorkshopSearchPaths(gameInfo, itemDirectories);

            Console.WriteLine();
            ConsoleHelpers.WriteLine($"Mounted {itemDirectories.Length} workshop items!", ConsoleColor.Green);
        }
    }
}
