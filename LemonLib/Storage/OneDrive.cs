using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LemonLib.Storage
{
    public class OneDrive
    {

        public struct Quota
        {
            public long Total, Used, Remaining, App;
        };

        public struct File
        {
            public string Id;
            public string Name;
            public DateTime DateTime;
        }

        private static string AppId;
        private static string[] Scopes;

        public static string LOCAL_TOKEN_KEY = "RTOKEN";

        private static AccountSession Session = null;
        private static IOneDriveClient Client;

        public static bool IsLogged
        {
            get
            {
                return Session != null;
            }
        }


        public static void Initialize(string appId, params string[] scopes)
        {
            AppId = appId;
            Scopes = scopes;
        }

        public static async Task LoginWhithoutUI()
        {
            try
            {
                Client = await OneDriveClient.GetSilentlyAuthenticatedMicrosoftAccountClient(AppId, "", Scopes, ApplicationData.Current.LocalSettings.Values[LOCAL_TOKEN_KEY] as string);
                Session = Client.AuthenticationProvider.CurrentAccountSession;
                ApplicationData.Current.LocalSettings.Values[LOCAL_TOKEN_KEY] = Session.RefreshToken;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public static async Task Login()
        {

            try
            {
                Client = OneDriveClientExtensions.GetClientUsingWebAuthenticationBroker(AppId, Scopes);
                Session = await Client.AuthenticateAsync();
                ApplicationData.Current.LocalSettings.Values[LOCAL_TOKEN_KEY] = Session.RefreshToken;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Session = null;
            }
        }

        public static async Task Logout()
        {
            if (Client == null || !IsLogged)
                return;
            await Client.SignOutAsync();
            ApplicationData.Current.LocalSettings.Values.Remove(LOCAL_TOKEN_KEY);
            Session = null;
        }

        public static async Task Upload(StorageFile file, string name = null)
        {
            if (IsLogged)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var item = await Client.Drive.Special.AppRoot
                        .ItemWithPath(name == null ? file.Name : name)
                        .Content.Request()
                        .PutAsync<Item>(stream);
                }
            }
        }

        public static async Task Delete(string name)
        {
            if (IsLogged)
            {
                await Client.Drive.Special.AppRoot
                    .ItemWithPath(name).Request().DeleteAsync();
            }
        }

        public static async Task CreateFolder(string name)
        {
            if (IsLogged)
            {
                var newFolder = new Item
                {
                    Name = name,
                    Folder = new Folder()
                };
                var newFolderCreated = await Client.Drive.Special.AppRoot
                  .Children
                  .Request()
                  .AddAsync(newFolder);
            }
        }

        public static async Task<Stream> Download(string name)
        {
            if (IsLogged)
            {
                return await Client.Drive.Special.AppRoot
                    .ItemWithPath(name)
                    .Content.Request()
                    .GetAsync();
            }
            return null;
        }

        public static async Task<File[]> GetFiles(string folder)
        {
            var filesList = await Client.Drive.Special.AppRoot.ItemWithPath(folder).Children.Request().GetAsync();
            List<File> files = new List<File>();
            foreach (var fileL in filesList)
            {
                var file = new File();
                file.Id = fileL.Id;
                file.Name = fileL.Name;
                file.DateTime = (fileL.LastModifiedDateTime ?? DateTimeOffset.MinValue).LocalDateTime;
                files.Add(file);
            }
            return files.ToArray();
        }

        public static async Task<Quota?> GetQuota()
        {
            if (IsLogged)
            {
                var drive = await Client.Drive.Request().GetAsync();
                Quota quota = new Quota();
                quota.Remaining = drive.Quota.Remaining ?? 0;
                quota.Total = drive.Quota.Total ?? 0;
                quota.Used = drive.Quota.Used ?? 0;
                var appRoot = await Client.Drive.Special.AppRoot.Request().GetAsync();
                quota.App = appRoot.Size ?? 0;
                return quota;
            }
            return null;
        }
    }
}
