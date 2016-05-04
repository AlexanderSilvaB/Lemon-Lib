using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace LemonLib.Documents
{
    public class PDF
    {
        public enum Errors { NoError, InvalidFile, InvalidPassword, Unknown };
        public struct PageOptions
        {
            public Color? BackgroundColor { get; set; }
            public uint? Width { get; set; }
            public uint? Height { get; set; }
            public Rect? SourceRect { get; set; }
        }
        public struct PageDimensions
        {
            public Rect ArtBox { get; set; }
            public Rect BleedBox { get; set; }
            public Rect CropBox { get; set; }
            public Rect MediaBox { get; set; }
            public Rect TrimBox { get; set; }
        }
        public struct PageInfo
        {
            public bool Valid { get; set; }
            public uint Index { get; set; }
            public uint Rotation { get; set; }
            public float Zoom { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public PageDimensions Dimensions;
        }



        private const int WrongPassword = unchecked((int)0x8007052b); // HRESULT_FROM_WIN32(ERROR_WRONG_PASSWORD) 
        private const int GenericFail = unchecked((int)0x80004005);   // E_FAIL 

        private PdfDocument pdfDocument;

        public uint PagesCount
        {
            get
            {
                if (!IsLoaded)
                    return 0;
                return pdfDocument.PageCount;
            }
        }

        public bool IsProtected
        {
            get
            {
                if (!IsLoaded)
                    return false;
                return pdfDocument.IsPasswordProtected;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return pdfDocument != null;
            }
        }

        public async Task<Errors> Load(StorageFile file, string password = null)
        {
            try
            {
                if (password == null)
                    pdfDocument = await PdfDocument.LoadFromFileAsync(file);
                else
                    pdfDocument = await PdfDocument.LoadFromFileAsync(file, password);
            }
            catch (Exception ex)
            {
                switch (ex.HResult)
                {
                    case WrongPassword:
                        return Errors.InvalidPassword;
                    case GenericFail:
                        return Errors.InvalidFile;
                    default:
                        return Errors.Unknown;
                }
            }

            if (pdfDocument == null)
            {
                return Errors.Unknown;
            }

            return Errors.NoError;
        }

        public PageInfo GetPageInfo(uint pageIndex)
        {
            PageInfo info = new PageInfo();
            if (!IsLoaded || pageIndex < 1 || pageIndex > PagesCount)
            {
                info.Valid = false;
            }
            else
            {
                using (PdfPage page = pdfDocument.GetPage(pageIndex))
                {
                    info.Valid = true;
                    info.Index = page.Index;
                    info.Width = page.Size.Width;
                    info.Height = page.Size.Height;
                    info.Rotation = 90 * (uint)page.Rotation;
                    info.Zoom = page.PreferredZoom;
                    info.Dimensions = new PageDimensions();
                    info.Dimensions.ArtBox = page.Dimensions.ArtBox;
                    info.Dimensions.BleedBox = page.Dimensions.BleedBox;
                    info.Dimensions.CropBox = page.Dimensions.CropBox;
                    info.Dimensions.MediaBox = page.Dimensions.MediaBox;
                    info.Dimensions.TrimBox = page.Dimensions.TrimBox;
                }
            }
            return info;
        }

        public async Task<IRandomAccessStream> GetPageStream(uint pageIndex)
        {
            if (!IsLoaded || pageIndex < 0 || pageIndex >= PagesCount)
            {
                return null;
            }
            using (PdfPage page = pdfDocument.GetPage(pageIndex))
            {
                var stream = new InMemoryRandomAccessStream();
                await page.RenderToStreamAsync(stream);
                return stream;
            }
        }

        public async Task<IRandomAccessStream> GetPageStream(uint pageIndex, PageOptions options)
        {
            if (!IsLoaded || pageIndex < 0 || pageIndex >= PagesCount)
            {
                return null;
            }
            using (PdfPage page = pdfDocument.GetPage(pageIndex))
            {
                PdfPageRenderOptions opc = new PdfPageRenderOptions();
                if (options.BackgroundColor != null)
                    opc.BackgroundColor = options.BackgroundColor.Value;
                if (options.Width != null)
                    opc.DestinationWidth = options.Width.Value;
                if (options.Height != null)
                    opc.DestinationHeight = options.Height.Value;
                if (options.SourceRect != null)
                    opc.SourceRect = options.SourceRect.Value;
                var stream = new InMemoryRandomAccessStream();
                await page.RenderToStreamAsync(stream, opc);
                return stream;
            }
        }

        public async Task<BitmapImage> GetPage(uint pageIndex)
        {
            IRandomAccessStream stream = await GetPageStream(pageIndex);
            BitmapImage src = new BitmapImage();
            await src.SetSourceAsync(stream);
            return src;
        }

        public async Task<BitmapImage> GetPage(uint pageIndex, PageOptions options)
        {
            IRandomAccessStream stream = await GetPageStream(pageIndex, options);
            BitmapImage src = new BitmapImage();
            await src.SetSourceAsync(stream);
            return src;
        }
    }
}
