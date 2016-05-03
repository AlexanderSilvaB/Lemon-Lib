using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LemonLib.Util
{
    public class ClipBoard
    {

        public static void Clear()
        {
            Windows.ApplicationModel.DataTransfer.Clipboard.Clear();
        }

        public static void OnContentChanged(Action contentChanged)
        {
            Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += (s, e) => contentChanged();
        }

        public static void SetText(string text)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetRtf(string text)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetRtf(text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetHtml(string text)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetHtmlFormat(text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetWebUri(Uri uri)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetWebLink(uri);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetApplicationUri(Uri uri)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetApplicationLink(uri);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetStorageItem(IStorageItem item)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            IStorageItem[] files = new IStorageItem[] { item };
            dataPackage.SetStorageItems(files);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetStorageItems(IEnumerable<IStorageItem> items)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetStorageItems(items);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetImage(StorageFile file)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetImage(IRandomAccessStream stream)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static void SetImage(Uri uri)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromUri(uri));
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        public static async Task<string> GetText()
        {
            string text = null;
            Windows.ApplicationModel.DataTransfer.DataPackageView dataPackage = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if(dataPackage != null)
            {
                
                try
                {
                    text = await dataPackage.GetTextAsync();
                }
                catch { }
            }
            return text;
        }

        public static async Task<string> GetRtf()
        {
            string text = null;
            Windows.ApplicationModel.DataTransfer.DataPackageView dataPackage = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackage != null)
            {

                try
                {
                    text = await dataPackage.GetRtfAsync();
                }
                catch { }
            }
            return text;
        }

        public static async Task<string> GetHtml()
        {
            string text = null;
            Windows.ApplicationModel.DataTransfer.DataPackageView dataPackage = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackage != null)
            {

                try
                {
                    text = await dataPackage.GetHtmlFormatAsync();
                }
                catch { }
            }
            return text;
        }

        public static async Task<Uri> GetWebUri()
        {
            Uri uri = null;
            Windows.ApplicationModel.DataTransfer.DataPackageView dataPackage = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackage != null)
            {

                try
                {
                    uri = await dataPackage.GetWebLinkAsync();
                }
                catch { }
            }
            return uri;
        }

        public static async Task<Uri> GetApplicationUri()
        {
            Uri uri = null;
            Windows.ApplicationModel.DataTransfer.DataPackageView dataPackage = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackage != null)
            {

                try
                {
                    uri = await dataPackage.GetApplicationLinkAsync();
                }
                catch { }
            }
            return uri;
        }

        public static async Task<IReadOnlyList<IStorageItem>> GetStorageFiles()
        {
            IReadOnlyList<IStorageItem> files = null;
            Windows.ApplicationModel.DataTransfer.DataPackageView dataPackage = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackage != null)
            {

                try
                {
                    files = await dataPackage.GetStorageItemsAsync();
                }
                catch { }
            }
            return files;
        }

        public static async Task<IRandomAccessStreamReference> GetBitmap()
        {
            IRandomAccessStreamReference stream = null;
            Windows.ApplicationModel.DataTransfer.DataPackageView dataPackage = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackage != null)
            {

                try
                {
                    stream = await dataPackage.GetBitmapAsync();
                }
                catch { }
            }
            return stream;
        }
    }
}
