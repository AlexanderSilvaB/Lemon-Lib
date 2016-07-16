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

using Windows.UI.Popups;
using System.Threading.Tasks;
using LemonLib.Devices.Bluetooth.Serial;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Examples.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BluetoothRFCOMMPage : Page
    {
        Client BluetoothClient = null;
        Server BluetoothServer = null;
        string uuid = "34B1CF4D-1069-4AD6-89B6-E161D79BE4D8";

        public BluetoothRFCOMMPage()
        {
            this.InitializeComponent();

            /*
             * Add bluetooth and radios DeviceCapability to Capabilities in Package.appxmanifest
             */
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(await LemonLib.Devices.Bluetooth.Device.RequestBluetoothAccess())
            {
                if (await LemonLib.Devices.Bluetooth.Device.TurnBluetooth(true))
                {
                    
                }
                else
                {
                    await ShowMessage("Could not enable bluetooth");
                }
            }
            else
            {
                await ShowMessage("Could not use bluetooth");
            }

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var devices = await LemonLib.Devices.Bluetooth.Device.GetDevices(uuid);
            DevicesList.ItemsSource = devices;
        }

        private async void DevicesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            LemonLib.Devices.Bluetooth.Device device = e.ClickedItem as LemonLib.Devices.Bluetooth.Device;
            await BluetoothClient.Connect(device);
            if (BluetoothClient.IsConnected)
            {
                Log("Connected to " + device.Name + "[" + device.Address + "]");
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

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (BluetoothClient != null)
                BluetoothClient.Disconnect();
            BluetoothClient = null;
            BluetoothServer = new Server("Bluetooth service", uuid);
            BluetoothServer.DataMode = LemonLib.Devices.Bluetooth.Device.DataModes.String;
            BluetoothServer.OnReceive += OnReceive;
            BluetoothServer.OnConnect += BluetoothServer_OnConnect;
            if(await BluetoothServer.Start())
            {
                Log("Server running");
            }
            else
            {
                await ShowMessage("Could not start server");
                BluetoothServer = null;
            }
        }

        private void BluetoothServer_OnConnect(object sender, LemonLib.Devices.Bluetooth.ServerConnectionEventArgs e)
        {
            Log("Connected to "+e.Name+"["+e.Address+"]");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (BluetoothServer != null)
                BluetoothServer.Disconnect();
            BluetoothServer = null;
            BluetoothClient = new Client("Bluetooth service");
            BluetoothClient.DataMode = LemonLib.Devices.Bluetooth.Device.DataModes.String;
            BluetoothClient.OnReceive += OnReceive;
            Log("Client running");
            Button_Click_1(null, null);
        }

        private async void OnReceive(object sender, LemonLib.Devices.Bluetooth.SerialEventArgs e)
        {
            if(e.Message != null)
                Log("Received: "+e.Message);
            else if(e.Bytes != null)
            {
                Log("Received: "+(char)e.Bytes[0]);
            }
            else if(e.Error != null)
            {
                await ShowMessage(e.Error.Message);
            }
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if(BluetoothClient != null && BluetoothClient.IsConnected)
            {
                await BluetoothClient.Write(send.Text);
                Log("Sent: "+send.Text);
            }
            else if(BluetoothServer != null && BluetoothServer.IsConnected)
            {
                await BluetoothServer.Write(send.Text);
                Log("Sent: " + send.Text);
            }
            else
            {
                await ShowMessage("Could not send, not connected!");
            }
        }

        private async void Log(object obj)
        {
            await text.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (obj != null)
                    text.Text = obj.ToString() + "\n" + text.Text;
            });
        }
    }
}
