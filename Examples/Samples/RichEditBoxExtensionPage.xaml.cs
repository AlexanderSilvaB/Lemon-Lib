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

using LemonLib.Extensions;
using LemonLib.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RichEditBoxExtensionPage : Page
    {
        public RichEditBoxExtensionPage()
        {
            this.InitializeComponent();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await EditBox.Open();
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            await EditBox.Save();
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            EditBox.Bold();
        }

        private async void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            var file = await Files.OpenFileDialog(RichEditBoxExtension.ImageTypes);
            await EditBox.Insert(file);
        }
    }
}
