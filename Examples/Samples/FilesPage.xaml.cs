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

using LemonLib.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilesPage : Page
    {
        public FilesPage()
        {
            this.InitializeComponent();
            Files.Initialize();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Files.CreateFile("Local:\\Files\\" + DateTime.Now.ToFileTimeUtc() + ".txt");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var files = await Files.GetFiles("Local:\\Files");
            string list = "Local: \n";
            foreach(var file in files)
            {
                list += file.Name + "\n";
            }
            files = await Files.GetFiles("Roaming:\\Files");
            list += "\nRoaming: \n";
            foreach (var file in files)
            {
                list += file.Name + "\n";
            }
            FilesList.Text = list;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await Files.CreateFile("Roaming:\\Files\\" + DateTime.Now.ToFileTimeUtc() + ".txt");
        }
    }
}
