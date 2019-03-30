using System.IO;
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
            var gameInfo = _gameInfoRepository.FindGameInfo();

            var items = await _workshopRepository.GetUserSubscribedItems(token);

            var itemDirectories = items.Where(x => x.Installed).Select(x => x.Directory);

            await _gameInfoRepository.SetWorkshopSearchPaths(gameInfo, itemDirectories);
        }
    }
}
