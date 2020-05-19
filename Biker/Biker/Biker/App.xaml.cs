using Biker.Views;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Xamarin.Forms;
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
            DownloadZip("https://manadevfrom.blob.core.windows.net/zips/biker.zip");

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

        private async void HandleNotificationReceived(OSNotification result) 
        {
           await MainPage.DisplayAlert("Noti","HandleNotificationReceived","X");
        }
        private async void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            await MainPage.DisplayAlert("Noti", "HandleNotificationOpened", "X");
        }

    }

}
