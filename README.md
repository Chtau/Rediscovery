# Rediscovery

Rediscovery allows you to remote execute functions on desktops and Android phone from another device.

The available functions on the remote device and the control UI on Android are all plugin based.
With the plugin system everybody with a little coding knowledge should be able to create functions and a UI to easy remote execute any task.
Execute Android functions from a desktop is not integrated yet, but it will come.


Feedback and Contributions are very welcome!

### Goal

The Goal of Rediscovery is to create a Bi directional remote control Crossplatform software with the ability of ease to extend.

### Technology and Frameworks

* C#
* .NET Core
* Xamarin
* Avalonia
* Grpc
* JavaScript
* HTML
* Sqlite

## Desktop Applications

### Control Panel

![Control Panel](./Resources/Docs/Control_Panel.png)

The control panel is as the name says the central control of the desktop services and UI at the moment.

### Manager GUI

![Manager GUI active devices](./Resources/Docs/ManagerGUI_Active_Devices.png)

Manager GUI shows informations about devices connected to the service, logs and saved devices.

### Manager

The manager should have pretty much the same functions the Manager GUI but in the command line.

### Service

Main service to provide all features and is required run if you want to use any other desktop application.

After the service is started it will update the settings for Manager, ManagerGUI and Discovery service with the public IP Address and adds a Firewall Rule (Firewall rule may require Administration privileges).
Settings of the other applications can only be updated if they are in specific folders in the same directory as the service folder (See default folder structure from releases).
If you use a different structure you can set the paths to the other applications in the service settings (Settings Key:`RemoteResourceSettings`).


### Discovery Service

This service allows you to use the discovery function on the Android application and remove the need to manual configuration the desktop on Android.

After starting the Discovery service will add a Firewall rule (this may require Administration privileges).

## Android

![Android Start](./Resources/Docs/Android_Start.png)

### Connect

![Android device found](./Resources/Docs/Android_Discover_device_found.jpg)
![Android device found](./Resources/Docs/Android_Device_connected.png)

### Features

![Android device found](./Resources/Docs/Android_Features.png)
![Android device found](./Resources/Docs/Android_MediaPlayer.png)
![Android device found](./Resources/Docs/Android_Terminal.png)
![Android device found](./Resources/Docs/Android_Send_File.png)


## Upcoming

* Call Android features from Desktop via Plugins
* Linux Release
* More Plugins
* Plugin Manager
* More Documentation

## Infrastructur

### Host
A Host manages communication between connected devices.
Hosts can also provide features for device to consume.

### Device
Devices provide and consume features from and to other devices.

## Communication
**Device** connects to a **Host**, now the connected **Device** can see all other connected **Devices**. If the **Host** provides any features by it self the **Device** can already access it. **Device** features can only be accessed after the access right to the **Device** is being given. A **Host** can dictate that all **Devices** connected can access all features of all **Devices** all the time.


## License

[Mozilla Public License Version 2.0](LICENSE) � [Christoph Taucher](https://github.com/Chtau)