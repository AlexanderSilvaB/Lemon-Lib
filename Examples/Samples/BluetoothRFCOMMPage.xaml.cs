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

using LemonLib.Devices;
using Windows.UI.Popups;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BluetoothRFCOMMPage : Page
    {
        BluetoothRfcomm BluetoothClient;
        public BluetoothRFCOMMPage()
        {
            this.InitializeComponent();

            /*
             * Add bluetooth and radios DeviceCapability to Capabilities in Package.appxmanifest
             */

            BluetoothClient = new BluetoothRfcomm("Bluetooth service");
        }
        

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (await BluetoothClient.RequestBluetoothAccess())
            {
                await ShowMessage("Bluetooth use allowed");
            }
            else
            {
                await ShowMessage("Could not use bluetooth");
            }
        }

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            bool IsOn = (sender as ToggleSwitch).IsOn;
            if (IsOn)
            {
                if(await BluetoothClient.TurnBluetooth(true))
                {
                    await ShowMessage("Bluetooth enabled");
                }
                else
                {
                    (sender as ToggleSwitch).IsOn = false;
                    await ShowMessage("Could not enable bluetooth");
                }
            }
            else
            {
                if (await BluetoothClient.TurnBluetooth(false))
                {
                    await ShowMessage("Bluetooth disabled");
                }
                else
                {
                    (sender as ToggleSwitch).IsOn = true;
                    await ShowMessage("Could not disable bluetooth");
                }
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var devices = await BluetoothClient.GetDevices();
            DevicesList.ItemsSource = devices;
        }

        private async void DevicesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            BluetoothDevice device = e.ClickedItem as BluetoothDevice;
            await BluetoothClient.Connect(device);
            if (BluetoothClient.IsConnected)
            {
                await ShowMessage("Connect");
            }
            else
            {
                await ShowMessage("Could not connect");
            }
        }

        private async Task ShowMessage(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}
