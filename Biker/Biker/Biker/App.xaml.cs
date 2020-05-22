using Biker.Models;
using Biker.Services;
using Biker.Views;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Biker
{
    public partial class App : Application
    {
        public static bool IsInForeground;
        private const string MCLocalStorageFolderName = "mcontent";
        private readonly string destinationFolder;

        public App()
        {
            InitializeComponent();

            var dirPath = Environment.SpecialFolder.LocalApplicationData;
            var defaultDirPath = Environment.GetFolderPath(dirPath);
            destinationFolder = Path.Combine(defaultDirPath, MCLocalStorageFolderName);
            DownloadZip("https://manadevfrom.blob.core.windows.net/zips/zip-biker.zip");

            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
            IsInForeground = true;
            OneSignal.Current.StartInit("bd24274c-4778-491a-b45f-4185b2a0110f")//TODO: insert ID here
                .InFocusDisplaying(Com.OneSignal.Abstractions.OSInFocusDisplayOption.None)
                .HandleNotificationReceived(HandleNotificationReceived)
                .HandleNotificationOpened(HandleNotificationOpened)
                .EndInit();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void DownloadZip(string DownloadFileURL)
        {

            if (Directory.Exists($"{destinationFolder}"))
            {
                Directory.Delete($"{destinationFolder}", true);
            }
            var data = new WebClient().DownloadData(new Uri(DownloadFileURL));
            ExtractZip(data);
        }

        private void ExtractZip(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var archive = new ZipArchive(stream))
            {
                archive.ExtractToDirectory(destinationFolder);
            }
        }

        private void HandleNotificationReceived(OSNotification result)
        {
            ProcessNotification(result.payload.additionalData);
        }
        private void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            ProcessNotification(result.notification.payload.additionalData);
        }

        public void ProcessNotification(IDictionary<string, object> notificationData)
        {
            var isValidNotiMessage = notificationData != null && notificationData.Any();
            if (isValidNotiMessage)
            {
                notificationData.ForEach(it =>
                {
                    NotificationService.AddNotificationStack(it.Key, it.Value);
                    NotificationService.PublishNotification(it.Key);
                });
            }
        }
    }
}
