# LemonLib
A powerfull library for Universal Windows Platform (UWP)
<br/>
<a href="http://alexandersilvab.github.io/Lemon-Lib/">Website</a>
<br/>

##Namespaces

<b>Helpers</b> - Provides helpers for common UWP tasks
<br/>
<b>Storage</b> - Provides classes for storage tasks
<br/>
<b>Devices</b> - Provides classes for devices manipulation
<br/>
<b>Util</b> - Provides utility classes for UWP apps
<br/>
<b>Documents</b> - Provides classes to manipulate documents
<br/>
<b>Extensions</b> - Provides extensions other UWP classes and controls
<br/>
<b>Web</b> - Classes to simplify web content manipulation

##Classes

<b>Helpers.ColorHelper</b> - Set of methods to convert Color to Hex string and string to Color
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
<b>Documents.PDF</b> - Provives an easy way open and show pdf files
<br/>
<b>Extensions.ColorExtension</b> - Extension to convert Color to Hex string and string to Color
<br/>
<b>Extensions.RichEditBoxExtension</b> - Extension to manipulate RichEditBox. With this extension you can add images, format, open and save RTF documents in RichEditBox.
<br/>
<b>Web.HttpContent</b> - Represents a container for web contents, you can use it to store data for a post request, upload files, cookies and much more.
<br/>
<b>Web.HttpResponse</b> - Represents the response from a server after some request. It contains the request code, the returned data, exceptions and cookies/headers.
<br/>
<b>Web.HttpRequester</b> - This class provides an easy way to make http requests like post, get, put or delete. It provides the interface to add cookies and headers such as authorization, user-agent, content-type or any other header you need.
<br/>
<b>Web.RestFul</b> - This class simplifies the manipulation of a RESTFul server, it easily allows you to make requests to an endpoint and passing any data you need. This class uses the class Web.HttpRequester as base.
<br/>

##Examples

The folder Examples contains a VS2015 project with samples for the LemonLib classes
<br/>
<b>Examples screenshots</b>
<br/>
<img src="https://github.com/AlexanderSilvaB/Lemon-Lib/raw/master/Screenshots/Desktop.png" height="350px"/>
<img src="https://github.com/AlexanderSilvaB/Lemon-Lib/raw/master/Screenshots/Mobile.png" height="350px"/>

## Contributing

1. Fork it
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin my-new-feature`)
5. Create new Pull Request

Obs.: To keep LemonLib consitent, please add your classes in a group inside the LemonLib namespace, like LemonLib.Storage or LemonLib.Devices. Adding subgroups is also a good practice.

## License
LemonLib is licensed under the MIT license. (http://opensource.org/licenses/MIT)
