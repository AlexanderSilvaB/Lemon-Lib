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

using LemonLib.Web;
using LemonLib.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RestFulPage : Page
    {
        RestFul rest;
        public RestFulPage()
        {
            this.InitializeComponent();
        }

        private async void actionBt_Click(object sender, RoutedEventArgs e)
        {
            actionBt.IsEnabled = false;
            try
            {
                rest = new RestFul(new Uri(serverTb.Text));
                Response response = null;
                switch(requestCb.SelectedIndex)
                {
                    case 0:
                        response = await rest.GetAsync(endpointTb.Text);
                        break;
                    case 1:
                        Content data = new Content();
                        data.Add("title", "Title test");
                        data.Add("body", "Body test");
                        response = await rest.PostAsync(endpointTb.Text, data);
                        break;
                    case 2:
                        response = await rest.PutAsync(endpointTb.Text);
                        break;
                    case 3:
                        response = await rest.DeleteAsync(endpointTb.Text);
                        break;
                }
                if(response != null)
                {
                    if(response.Exception != null)
                    {
                        Log(response.Exception.Message);
                    }
                    else
                    {
                        Log(response.GetString());
                    }
                }
            }
            catch(Exception ex)
            {
                Log(ex.Message);
            }
            actionBt.IsEnabled = true;
        }

        private void Log(string text)
        {
            terminal.Text = ">\n" + text + "\n\n" + terminal.Text;
        }
    }
}
