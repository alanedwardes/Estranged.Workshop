using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Estranged.Workshop
{
    internal sealed class WorkshopSynchroniser
    {
        private readonly WorkshopRepository _workshopRepository;
        private readonly GameInfoRepository _gameInfoRepository;

        public WorkshopSynchroniser(WorkshopRepository repository, GameInfoRepository gameInfoRepository)
        {
            _workshopRepository = repository;
            _gameInfoRepository = gameInfoRepository;
        }

        public async Task Synchronise(CancellationToken token)
        {
            Console.WriteLine("Mounting downloaded workshop items...");

            var gameInfo = _gameInfoRepository.FindGameInfo();

            var items = await _workshopRepository.GetUserSubscribedItems(token);

            var itemDirectories = items.Where(x => x.Installed).Select(x => x.Directory).ToArray();

            await _gameInfoRepository.SetWorkshopSearchPaths(gameInfo, itemDirectories);

            Console.WriteLine();
            ConsoleHelpers.WriteLine($"Mounted {itemDirectories.Length} workshop items!", ConsoleColor.Green);
        }
    }
}
