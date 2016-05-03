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

using LemonLib.Helpers;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UIPage : Page
    {
        public UIPage()
        {
            this.InitializeComponent();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch btn = sender as ToggleSwitch;
            UI.ExtendView(btn.IsOn);
        }

        private void ToggleSwitch_Toggled_1(object sender, RoutedEventArgs e)
        {
            ToggleSwitch btn = sender as ToggleSwitch;
            if(btn.IsOn)
            {
                UI.SetTitleBarColor(Colors.Red, Colors.White, Colors.PaleVioletRed, Colors.Gray, UI.DeviceType.Both);
                UI.SetTitleBarButtonsColor(Colors.Red, Colors.White, UI.ButtonState.Normal);
            }
            else
            {
                UI.SetTitleBarColor(UI.AccentColor, Colors.White, UI.GetColor(UI.UIColorType.AccentDark1), UI.GetColor(UI.UIColorType.AccentDark2), UI.DeviceType.Both);
                UI.SetTitleBarButtonsColor(UI.AccentColor, Colors.White, UI.ButtonState.Normal);
            }
        }

        private void ToggleSwitch_Toggled_2(object sender, RoutedEventArgs e)
        {
            ToggleSwitch btn = sender as ToggleSwitch;
            if(!UI.SetFullScreen(btn.IsOn))
            {
                btn.IsOn = !btn.IsOn;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AccentColor.Background = new SolidColorBrush(UI.AccentColor);
            UI.OnColorsChange(OnColorsChange);
        }

        private void OnColorsChange()
        {
            AccentColor.Background = new SolidColorBrush(UI.AccentColor);
        }
    }
}
