# Lemon-Lib
A powerfull library for Universal Windows Platform (UWP)

<h4>Namespaces</h4>

<b>Helpers</b> - Provides helpers for common UWP tasks
<br/>
<b>Storage</b> - Provides classes for storage tasks
<br/>
<b>Devices</b> - Provides classes for devices manipulation
<br/>
<b>Util</b> - Provides utility classes for UWP apps

<h4>Classes</h4>

<b>Helpers.ColorHelper</b> - Set of extensions to convert Color to Hex string and string to Color
<br/>
<b>Helpers.BackgroundTaskHelper</b> - Provides methods to easily register and unregister background tasks
<br/>
Obs.: To use this helper you need create a project for the background task and add it to package.appxmanifest
<br/>
<b>Helpers.UI</b> - Provides methods manage the app UI such as TitleBar, TitleBarButtons, AccentColor, ...
<br/>
<b>Storage.Files</b> - Provides a nice path based way to read and write files in local and roaming directory
<br/>
<b>Storage.OneDrive</b> - Provides methods to login, upload, download, delete and many other actions in OneDrive
<br/>
Obs.: This class needs that before you install Microsoft.OneDrive.SDK which can be obtained over NuGet
<br/>
<b>Devices.BluetoothDevice</b> - Represents a bluetooth device
<br/>
<b>Devices.BluetoothRfcomm</b> - Provives an easy and intuitive interface to comunicate with a rfcomm bluetooth device
<br/>
Obs.: Add bluetooth and radios DeviceCapability to Capabilities in Package.appxmanifest
<br/>
<b>Util.Clipboard</b> - Provives an easy way to set and get content from the clipboard
<br/>


<h4>Examples</h4>

The folder Examples contains a VS2015 project with samples for the LemonLib classes
<br/>
<b>Examples screenshots</b>
![alt tag](https://github.com/AlexanderSilvaB/Lemon-Lib/raw/master/Screenshots/Desktop.png)
![alt tag](https://github.com/AlexanderSilvaB/Lemon-Lib/raw/master/Screenshots/Mobile.png)
