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

using LemonLib.Util;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClipBoardPage : Page
    {
        public ClipBoardPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClipBoard.SetText("Hello World!");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ClipBoard.OnContentChanged(OnContentChanged);
        }

        private async void OnContentChanged()
        {
            string text = await ClipBoard.GetText();
            if (text != null)
            {
                try
                {
                    ClipBoardHistoryTextBlock.Text = text + "\n\n" + ClipBoardHistoryTextBlock.Text;
                }
                catch { }
            }
        }
    }
}
