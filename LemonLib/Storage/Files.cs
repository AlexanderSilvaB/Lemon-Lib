﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace LemonLib.Storage
{
    public class Files
    {
        public const string LocalDrive = "Local:";
        public const string RoamingDrive = "Roaming:";
        public const string TempDrive = "Temp:";
        public const string AppxDrive = "Appx:";

        protected static StorageFolder Local = null;
        protected static StorageFolder Roaming = null;
        protected static StorageFolder Temp = null;
        protected static StorageFolder Appx = null;
        public static bool Initialized
        {
            get
            {
                return Local != null && Roaming != null && Temp != null && Appx != null;
            }
        }

        public static void Initialize()
        {
            Local = ApplicationData.Current.LocalFolder;
            Roaming = ApplicationData.Current.RoamingFolder;
            Temp = ApplicationData.Current.TemporaryFolder;
            Appx = Windows.ApplicationModel.Package.Current.InstalledLocation;
        }

        public static async Task<StorageFile> OpenFileDialog(params string[] extensions)
        {
            if (!Initialized)
                Initialize();
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            if (extensions.Length == 0)
                picker.FileTypeFilter.Add("*");
            else
                foreach (string extension in extensions)
                    picker.FileTypeFilter.Add(extension);
            var file = await picker.PickSingleFileAsync();
            return file;
        }

        public static async Task<StorageFile> OpenFileDialogWithThumbnails(params string[] extensions)
        {
            if (!Initialized)
                Initialize();
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.ViewMode = PickerViewMode.Thumbnail;
            if (extensions.Length == 0)
                picker.FileTypeFilter.Add("*");
            else
                foreach (string extension in extensions)
                    picker.FileTypeFilter.Add(extension);
            var file = await picker.PickSingleFileAsync();
            return file;
        }

        public static async Task<StorageFile> SaveFileDialog(params string[] extensions)
        {
            if (!Initialized)
                Initialize();
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            if (extensions.Length == 0)
                picker.FileTypeChoices.Add("All file types", new List<string> { "*" });
            else
            {
                foreach (string extension in extensions)
                    picker.FileTypeChoices.Add(extension + " file", new List<string> { extension });
                picker.DefaultFileExtension = extensions[0];
            }
            var file = await picker.PickSaveFileAsync();
            return file;
        }

        public static async Task CreateFolder(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);

            for (int i = 1; i < paths.Length; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
        }

        public static async Task<StorageFile> CreateFile(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length - 1; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
            return await folder.CreateFileAsync(paths[i], CreationCollisionOption.OpenIfExists);
        }

        public static async Task<StorageFile> CreateFileAndReplace(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length - 1; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
            return await folder.CreateFileAsync(paths[i], CreationCollisionOption.ReplaceExisting);
        }

        public static async Task DeleteFolder(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length - 1; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
            await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        public static async Task DeleteFile(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length - 1; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
            StorageFile file = await folder.GetFileAsync(paths[i]);
            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        public static async Task<StorageFolder> GetFolder(StorageFolder folder, string name)
        {
            if (!Initialized)
                Initialize();
            try
            {
                folder = await folder.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
            }
            catch(Exception ex)
            {
                var item = await folder.TryGetItemAsync(name);
                if(item.IsOfType(StorageItemTypes.Folder))
                {
                    folder = item as StorageFolder;
                }
                else
                {
                    folder = null;
                }
            }
            return folder;
        }

        public static async Task<bool> Exists(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length - 1; i++)
            {
                var item = await folder.TryGetItemAsync(paths[i]);
                if (item == null)
                {
                    return false;
                }
                else
                {
                    if(item.IsOfType(StorageItemTypes.Folder))
                        folder = item as StorageFolder;
                    else
                    {
                        return true;
                    }
                }
            }
            var item2 = await folder.TryGetItemAsync(paths[i]);
            if (item2 != null)
                return true;
            else
                return false;
        }

        public static async Task<StorageFolder> GetFolder(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length - 1; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
            return folder;
        }

        public static async Task<StorageFile> GetFile(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length - 1; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
            return await folder.GetFileAsync(paths[i]);
        }

        public static async Task<StorageFile[]> GetFiles(string name)
        {
            string[] paths;
            StorageFolder folder;
            GetStorage(name, out paths, out folder);
            int i = 1;
            for (i = 1; i < paths.Length; i++)
            {
                folder = await GetFolder(folder, paths[i]);
            }
            return (await folder.GetFilesAsync()).ToArray();
        }



        public static async Task<byte[]> ReadBytes(StorageFile file)
        {
            if (!Initialized)
                Initialize();
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }

        public static async Task<byte[]> ReadBytes(string fileName)
        {
            var file = await GetFile(fileName);
            return await ReadBytes(file);
        }

        public static async Task<string> Read(StorageFile file)
        {
            if (!Initialized)
                Initialize();
            return await FileIO.ReadTextAsync(file);
        }

        public static async Task<string> Read(string fileName)
        {
            var file = await GetFile(fileName);
            return await Read(file);
        }

        public static async Task Write(StorageFile file, string data)
        {
            if (!Initialized)
                Initialize();
            await FileIO.WriteTextAsync(file, data);
        }

        public static async Task Write(string fileName, string data)
        {
            var file = await GetFile(fileName);
            await Write(file, data);
        }

        public static async Task Write(StorageFile file, Stream data)
        {
            if (!Initialized)
                Initialize();
            using (var ostream = await file.OpenStreamForWriteAsync())
            {
                int count = 0;
                do
                {
                    var buffer = new byte[1024];
                    count = data.Read(buffer, 0, 1024);
                    await ostream.WriteAsync(buffer, 0, count);
                }
                while (data.CanRead && count > 0);
            }

        }

        public static async Task Write(string fileName, Stream data)
        {
            var file = await GetFile(fileName);
            await Write(file, data);
        }

        protected static string[] SplitPath(string name)
        {
            return name.Split('\\');
        }

        protected static void GetStorage(string path, out string[] paths, out StorageFolder folder)
        {
            if (!Initialized)
                Initialize();
            string[] pathsArray = SplitPath(path);
            List<string> pathsList = new List<string>(pathsArray);
            StorageFolder tmpFolder = Local;
            if (pathsList[0].Equals(RoamingDrive) || pathsList[0].Equals(LocalDrive) || pathsList[0].Equals(TempDrive) || pathsList[0].Equals(AppxDrive))
            {
                if (pathsList[0].Equals(RoamingDrive))
                {
                    tmpFolder = Roaming;
                }
                else if(pathsList[0].Equals(TempDrive))
                {
                    tmpFolder = Temp;
                }
                else if (pathsList[0].Equals(AppxDrive))
                {
                    tmpFolder = Appx;
                }
            }
            else
            {
                pathsList.Insert(0, LocalDrive);
            }
            paths = pathsList.ToArray();
            folder = tmpFolder;
        }
    }
}
