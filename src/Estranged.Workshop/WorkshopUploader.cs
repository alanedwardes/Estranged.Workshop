using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

        internal async Task Upload(DirectoryInfo uploadDirectory, CancellationToken token)
        {
            var item = await _workshopRepository.CreateItem(uploadDirectory, token);

            if (item.Error == null)
            {
                ConsoleHelpers.WriteLine("Item uploaded successfully!", ConsoleColor.Green);
                _browserOpener.OpenBrowser($"https://steamcommunity.com/sharedfiles/filedetails/?id={item.Id}");
                Console.WriteLine("You can edit the title, description, preview images and more from the item page.");
            }
            else
            {
                ConsoleHelpers.WriteLine($"Item upload failed. The error from Steam was '{item.Error}'.", ConsoleColor.Red);
                Console.WriteLine($"If you keep seeing this error, you may want to post on the forums: https://steamcommunity.com/app/{Constants.AppId}/discussions/");
            }
        }
    }
}
