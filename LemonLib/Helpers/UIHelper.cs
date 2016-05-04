using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace LemonLib.Helpers
{
    public class UIHelper
    {
        public enum UIColorType
        {
            Background = 0,
            Foreground = 1,
            AccentDark3 = 2,
            AccentDark2 = 3,
            AccentDark1 = 4,
            Accent = 5,
            AccentLight1 = 6,
            AccentLight2 = 7,
            AccentLight3 = 8,
            Complement = 9
        }

        public enum UIElementType
        {
            ActiveCaption = 0,
            Background = 1,
            ButtonFace = 2,
            ButtonText = 3,
            CaptionText = 4,
            GrayText = 5,
            Highlight = 6,
            HighlightText = 7,
            Hotlight = 8,
            InactiveCaption = 9,
            InactiveCaptionText = 10,
            Window = 11,
            WindowText = 12
        }

        public enum DeviceType
        {
            Both, Mobile, Desktop
        }

        public enum ButtonState
        {
            Normal, Pressed, Inactive, Hover
        }

        private static UISettings uiSettings = new UISettings();

        public static Color AccentColor
        {
            get
            {
                return uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            }
        }

        public static double TextScaleFactor
        {
            get
            {
                return uiSettings.TextScaleFactor;
            }
        }

        public static Color GetColor(UIColorType desiredColor)
        {
            return uiSettings.GetColorValue((Windows.UI.ViewManagement.UIColorType)(desiredColor));
        }

        public static Color GetColor(UIElementType desiredElement)
        {
            return uiSettings.UIElementColor((Windows.UI.ViewManagement.UIElementType)desiredElement);
        }

        public static void OnColorsChange(Action onColorsChange)
        {
            uiSettings.ColorValuesChanged += (s, a) => onColorsChange();
        }

        public static void OnTextScaleFactorChanged(Action onTextScaleFactorChanged)
        {
            uiSettings.TextScaleFactorChanged += (s, a) => onTextScaleFactorChanged();
        }

        public static void SetMinSize(double width = 320, double height = 320)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(width, height));
        }

        public static async void HideTitleBar(bool hide, DeviceType deviceType = DeviceType.Both)
        {
            if(deviceType == DeviceType.Both || deviceType == DeviceType.Desktop)
            {
                
            }
            if(deviceType == DeviceType.Both || deviceType == DeviceType.Mobile)
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    if(hide)
                        await StatusBar.GetForCurrentView().HideAsync();
                    else
                        await StatusBar.GetForCurrentView().ShowAsync();
                }
            }
        }

        public static void SetTitleBarOpacity(double opacity, DeviceType deviceType = DeviceType.Both)
        {
            if (deviceType == DeviceType.Both || deviceType == DeviceType.Desktop)
            {
                
            }
            if (deviceType == DeviceType.Both || deviceType == DeviceType.Mobile)
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    var statusBar = StatusBar.GetForCurrentView();
                    statusBar.BackgroundOpacity = opacity;
                }
            }
        }

        public static void ExtendView(bool extend)
        {
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(extend ? ApplicationViewBoundsMode.UseCoreWindow : ApplicationViewBoundsMode.UseVisible);
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = extend;
        }

        public static void SetTitleBarColor(Color backgroundColor, Color foregroundColor, Color inactiveBackgroundColor, Color inactiveForegroundColor, DeviceType deviceType = DeviceType.Both)
        {
            if (deviceType == DeviceType.Both || deviceType == DeviceType.Desktop)
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.BackgroundColor = backgroundColor;
                titleBar.ForegroundColor = foregroundColor;
                titleBar.InactiveBackgroundColor = inactiveBackgroundColor;
                titleBar.InactiveForegroundColor = inactiveForegroundColor;
            }
            if (deviceType == DeviceType.Both || deviceType == DeviceType.Mobile)
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    var statusBar = StatusBar.GetForCurrentView();
                    statusBar.BackgroundColor = backgroundColor;
                    statusBar.ForegroundColor = foregroundColor;
                    statusBar.BackgroundOpacity = 1;
                }
            }
        }

        public static void SetTitleBarButtonsColor(Color backgroundColor, Color foregroundColor, ButtonState state = ButtonState.Normal, DeviceType deviceType = DeviceType.Both)
        {
            if (deviceType == DeviceType.Both || deviceType == DeviceType.Desktop)
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                switch(state)
                {
                    case ButtonState.Normal:
                        titleBar.ButtonBackgroundColor = backgroundColor;
                        titleBar.ButtonForegroundColor = foregroundColor;
                        break;
                    case ButtonState.Pressed:
                        titleBar.ButtonPressedBackgroundColor = backgroundColor;
                        titleBar.ButtonPressedForegroundColor = foregroundColor;
                        break;
                    case ButtonState.Inactive:
                        titleBar.ButtonInactiveBackgroundColor = backgroundColor;
                        titleBar.ButtonInactiveForegroundColor = foregroundColor;
                        break;
                    case ButtonState.Hover:
                        titleBar.ButtonHoverBackgroundColor = backgroundColor;
                        titleBar.ButtonHoverForegroundColor = foregroundColor;
                        break;
                }
            }
            if (deviceType == DeviceType.Both || deviceType == DeviceType.Mobile)
            {
                
            }
        }


        public static bool IsFullScreen
        {
            get
            {
                return ApplicationView.GetForCurrentView().IsFullScreen;
            }
        }

        public static bool SetFullScreen(bool isFullScreen)
        {
            if(isFullScreen)
                return ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            return true;
        }

        public static async Task ShowMessageAsync(string message, string title = "")
        {
            MessageDialog dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }
    }
}
