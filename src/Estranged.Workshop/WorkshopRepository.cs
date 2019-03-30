using Facepunch.Steamworks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Facepunch.Steamworks.Workshop;

namespace Estranged.Workshop
{
    internal sealed class WorkshopRepository
    {
        private readonly Client _client;
        private readonly BrowserOpener _browserOpener;

        public WorkshopRepository(Client client, BrowserOpener browserOpener)
        {
            _client = client;
            _browserOpener = browserOpener;
        }

        public async Task<Item[]> GetUserSubscribedItems(CancellationToken token)
        {
            var waitCancellation = new CancellationTokenSource();

            var compositeCancellation = CancellationTokenSource.CreateLinkedTokenSource(waitCancellation.Token, token);

            var query = _client.Workshop.CreateQuery();
            query.UserId = _client.SteamId;
            query.UploaderAppId = _client.AppId;
            query.UserQueryType = UserQueryType.Subscribed;
            query.OnResult = x => waitCancellation.Cancel();
            query.Run();

            await Task.Delay(-1, compositeCancellation.Token)
                .ContinueWith(x => x, TaskContinuationOptions.OnlyOnCanceled);

            return query.Items;
        }

        public async Task<Editor> CreateItem(DirectoryInfo uploadDirectory, CancellationToken token)
        {
            return await UpdateItem(null, uploadDirectory, token);
        }

        public async Task<Editor> UpdateItem(ulong? itemId, DirectoryInfo uploadDirectory, CancellationToken token)
        {
            var editor = itemId.HasValue ? _client.Workshop.EditItem(itemId.Value) : _client.Workshop.CreateItem(ItemType.Community);
            editor.Folder = uploadDirectory.FullName;
            editor.Publish();

            Console.WriteLine($"Publishing workshop item... this might take a little while.");

            while (editor.Publishing)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(20), token);
            }

            if (editor.NeedToAgreeToWorkshopLegal)
            {
                Console.WriteLine();
                ConsoleHelpers.WriteLine("Please agree to the Steam Workshop legal agreement.", ConsoleColor.Yellow);
                _browserOpener.OpenBrowser("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
            }

            return editor;
        }
    }
}
