using Facepunch.Steamworks;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Estranged.Workshop
{
    internal sealed class GameInfoRepository
    {
        private readonly Client _steam;

        public GameInfoRepository(Client steam)
        {
            _steam = steam;
        }

        public async Task SetWorkshopSearchPaths(FileInfo gameInfo, IEnumerable<DirectoryInfo> directories)
        {
            // Read the raw gameinfo.txt
            var rawVdf = await File.ReadAllTextAsync(gameInfo.FullName);

            VProperty gameInfoVdf = VdfConvert.Deserialize(rawVdf);

            var searchPathSection = (VObject)gameInfoVdf.Value["FileSystem"]["SearchPaths"];

            // Hack - the underlying type is a List<VProperty>
            var searchPaths = (List<VProperty>)searchPathSection.Children();

            // Remove all existing workshop entries
            searchPaths.RemoveAll(x => x.Key == "game+workshop");

            foreach (var directory in directories)
            {
                // Add all new workshop entries
                searchPaths.Insert(0, new VProperty("game+workshop", new VValue(directory.FullName)));
            }

            // Write out the new gameinfo.txt
            await File.WriteAllTextAsync(gameInfo.FullName, gameInfoVdf.ToString());
        }

        public FileInfo FindGameInfo()
        {
            var gameInfo = new FileInfo(Path.Combine(_steam.InstallFolder.FullName, "estrangedact1", "gameinfo.txt"));
            return gameInfo.Exists ? gameInfo : throw new InvalidOperationException($"gameinfo.txt not found in {gameInfo.FullName}");
        }
    }
}
