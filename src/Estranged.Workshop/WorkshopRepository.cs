using Steamworks;
using Steamworks.Ugc;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Estranged.Workshop
{
    internal sealed class WorkshopRepository
    {
        private readonly BrowserOpener _browserOpener;

        public WorkshopRepository(BrowserOpener browserOpener)
        {
            _browserOpener = browserOpener;
        }

        public async Task<Item[]> GetUserSubscribedItems(CancellationToken token)
        {
            var query = new Query().WhereUserSubscribed();

            var result = await query.GetPageAsync(1);

            return result.Value.Entries.ToArray();
        }

        public async Task<Item> GetItem(ulong itemId, CancellationToken token)
        {
            var query = new Query().WhereUserSubscribed().WithFileId(itemId);

            var result = await query.GetPageAsync(1);

            return result.Value.Entries.Single();
        }

        public async Task<PublishResult> UpdateItem(Item? item, DirectoryInfo uploadDirectory, CancellationToken token)
        {
            Editor editor;
            if (item.HasValue)
            {
                editor = new Editor(item.Value.Id);
            }
            else
            {
                editor = Editor.NewCommunityFile;
                editor.WithTitle("New Item");
                editor.WithDescription("Edit me");
            }

            var previewExtensions = new[] { ".png", ".jpeg", ".jpg" };

            var preview = uploadDirectory.EnumerateFiles()
                .Where(x => previewExtensions.Contains(x.Extension.ToLower()))
                .Where(x => x.Length < 1024 * 1024)
                .FirstOrDefault();

            editor.ForAppId(SteamClient.AppId);
            editor.WithContent(uploadDirectory);
            editor.WithPreviewFile(preview?.FullName);

            ConsoleHelpers.WriteLine();
            ConsoleHelpers.WriteLine($"Publishing workshop item... this might take a little while.");

            var result = await editor.SubmitAsync();
            if (result.NeedsWorkshopAgreement)
            {
                ConsoleHelpers.WriteLine();
                ConsoleHelpers.WriteLine("Please agree to the Steam Workshop legal agreement.", ConsoleColor.Yellow);
                _browserOpener.OpenBrowser("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
            }

            return result;
        }
    }
}
