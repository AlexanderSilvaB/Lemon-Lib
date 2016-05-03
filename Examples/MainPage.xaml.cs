using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using LemonLib.Helpers;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Examples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<PageItem> Pages;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UI.SetTitleBarColor(UI.AccentColor, Colors.White, UI.GetColor(UI.UIColorType.AccentDark1), UI.GetColor(UI.UIColorType.AccentDark2), UI.DeviceType.Both);
            UI.SetTitleBarButtonsColor(UI.AccentColor, Colors.White, UI.ButtonState.Normal);

            PageFrame.Navigate(typeof(HomePage));

            Pages = new List<PageItem>();

            Pages.Add(new PageItem() { Icon = "", Name = "ColorHelper", Page = typeof(Samples.ColorHelperPage) });
            //Pages.Add(new PageItem() { Icon = "", Name = "BackgroundTaskHelper", Page = typeof(Samples.BackgroundTaskHelperPage) });
            Pages.Add(new PageItem() { Icon = "", Name = "UI", Page = typeof(Samples.UIPage) });
            Pages.Add(new PageItem() { Icon = "", Name = "Files", Page = typeof(Samples.FilesPage) });
            Pages.Add(new PageItem() { Icon = "", Name = "OneDrive", Page = typeof(Samples.OneDrivePage) });
            Pages.Add(new PageItem() { Icon = "", Name = "BluetoothRfcomm", Page = typeof(Samples.BluetoothRFCOMMPage) });
            Pages.Add(new PageItem() { Icon = "", Name = "ClipBoard", Page = typeof(Samples.ClipBoardPage) });

            PagesListView.ItemsSource = Pages;


            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (PageFrame.CurrentSourcePageType != typeof(HomePage))
            {
                PageFrame.Navigate(typeof(HomePage));
                Title.Text = "LemonLib Examples";
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                PagesSplitView.IsPaneOpen = true;
                e.Handled = true;
            }
        }

        public void SetTitle(string title)
        {
            Title.Text = "LemonLib Examples - " + title;
        }

        private void PagesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PagesSplitView.IsPaneOpen = false;
            PageItem item = e.ClickedItem as PageItem;
            SetTitle(item.Name);
            PageFrame.Navigate(item.Page);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PagesSplitView.IsPaneOpen = !PagesSplitView.IsPaneOpen;
        }
    }
}
