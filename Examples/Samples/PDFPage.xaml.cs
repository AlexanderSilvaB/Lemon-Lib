using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using LemonLib.Documents;
using LemonLib.Storage;
using LemonLib.Helpers;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PDFPage : Page
    {
        PDF pdf = new PDF();
        public PDFPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var file = await Files.OpenFileDialog(".pdf");
            var error = await pdf.Load(file);
            if(error != PDF.Errors.NoError)
            {
                await UIHelper.ShowMessageAsync(error.ToString());
                return;
            }
            var page = await pdf.GetPage(0);
            Viewer.Source = page;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var file = await Files.OpenFileDialog(".pdf");
            var error = await pdf.Load(file);
            if (error != PDF.Errors.NoError)
            {
                await UIHelper.ShowMessageAsync(error.ToString());
                return;
            }
            var page = await pdf.GetPage(0, new PDF.PageOptions() { BackgroundColor = Colors.Gray });
            Viewer.Source = page;
        }
    }
}
