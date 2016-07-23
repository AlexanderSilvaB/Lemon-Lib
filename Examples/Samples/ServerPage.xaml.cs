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
using LemonLib.Web.Http;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServerPage : Page
    {
        Server server;
        public ServerPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            server = new Server();
            await server.Start();
            server.OnRequest = Server_OnRequest;
            webView.Navigate(server.GetAddress("/AnyPage.txt"));
        }

        private async Task Server_OnRequest(object sender, ServerRequest e)
        {
            e.Output = "Server<br/>";
            e.Output += DateTime.UtcNow.ToString();
            e.Output += "<br/>-------------------------";
            e.Output += "<br/>Path: " + e.Path;
            e.Handled = true;
            /*
            try
            {
                string filePath = "Data" + e.Path.Replace('/', '\\');
                IStorageItem file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetItemAsync(filePath);
                e.Output = await FileIO.ReadTextAsync(file as StorageFile);
                e.Handled = true;
            }
            catch (FileNotFoundException)
            {
                e.Handled = false;
            }
            */
        }
    }
}
