using Facepunch.Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            try
            {
                await Task.Delay(-1, compositeCancellation.Token);
            }
            catch when (waitCancellation.IsCancellationRequested)
            {
                // Got the query result
            }

            return query.Items;
        }

        public async Task<Item> GetItem(ulong itemId, CancellationToken token)
        {
            var waitCancellation = new CancellationTokenSource();

            var compositeCancellation = CancellationTokenSource.CreateLinkedTokenSource(waitCancellation.Token, token);

            var query = _client.Workshop.CreateQuery();
            query.UserId = _client.SteamId;
            query.UploaderAppId = _client.AppId;
            query.FileId = new List<ulong> { { itemId } };
            query.UserQueryType = UserQueryType.Followed;
            query.OnResult = x => waitCancellation.Cancel();
            query.Run();

            try
            {
                await Task.Delay(-1, compositeCancellation.Token);
            }
            catch when (waitCancellation.IsCancellationRequested)
            {
                // Got the query result
            }

            return query.Items.SingleOrDefault();
        }

        public async Task<Editor> UpdateItem(Item item, DirectoryInfo uploadDirectory, CancellationToken token)
        {
            Editor editor;
            if (item == null)
            {
                editor = _client.Workshop.CreateItem(ItemType.Community);
                editor.Title = "New Item";
                editor.Description = "Edit me";
            }
            else
            {
                editor = _client.Workshop.EditItem(item.Id);
            }

            var thumbnail = uploadDirectory.EnumerateFiles()
                .Where(x => x.Extension == ".png" || x.Extension == ".jpeg" || x.Extension == ".jpg")
                .Where(x => x.Length < 1024 * 1024)
                .FirstOrDefault();

            editor.WorkshopUploadAppId = _client.AppId;
            editor.Folder = uploadDirectory.FullName;
            editor.PreviewImage = thumbnail?.FullName;
            editor.Publish();

            ConsoleHelpers.WriteLine();
            ConsoleHelpers.WriteLine($"Publishing workshop item... this might take a little while.");

            while (editor.Publishing)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(20), token);
            }

            if (editor.NeedToAgreeToWorkshopLegal)
            {
                ConsoleHelpers.WriteLine();
                ConsoleHelpers.WriteLine("Please agree to the Steam Workshop legal agreement.", ConsoleColor.Yellow);
                _browserOpener.OpenBrowser("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
            }

            return editor;
        }
    }
}
