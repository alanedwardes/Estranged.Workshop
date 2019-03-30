using System;
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
