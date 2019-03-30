using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Facepunch.Steamworks.Workshop;

namespace Estranged.Workshop
{
    internal sealed class WorkshopUploader
    {
        private readonly WorkshopRepository _workshopRepository;
        private readonly BrowserOpener _browserOpener;

        public WorkshopUploader(WorkshopRepository workshopRepository, BrowserOpener browserOpener)
        {
            _workshopRepository = workshopRepository;
            _browserOpener = browserOpener;
        }

        internal async Task Upload(DirectoryInfo uploadDirectory, ulong? existingItemId, CancellationToken token)
        {
            if (!uploadDirectory.Exists)
            {
                ConsoleHelpers.FatalError($"Directory doesn't exist: {uploadDirectory.FullName}");
            }

            Item existingItem = null;
            if (existingItemId.HasValue)
            {
                existingItem = await _workshopRepository.GetItem(existingItemId.Value, token);
                if (existingItem == null)
                {
                    ConsoleHelpers.FatalError($"Item {existingItemId.Value} was not found.");
                }

                Console.WriteLine();
                Console.WriteLine($"Updating existing item '{existingItem.Title}'");
            }

            var item = await _workshopRepository.UpdateItem(existingItem, uploadDirectory, token);
            if (item.Error == null)
            {
                Console.WriteLine();
                ConsoleHelpers.WriteLine("Item uploaded successfully!", ConsoleColor.Green);

                Console.WriteLine();
                _browserOpener.OpenBrowser($"https://steamcommunity.com/sharedfiles/filedetails/?id={item.Id}");

                Console.WriteLine();
                ConsoleHelpers.WriteLine("You can edit the title, description, preview images and more from the item page.", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelpers.FatalError($"Item upload failed. The error from Steam was: {item.Error}");
            }
        }
    }
}
