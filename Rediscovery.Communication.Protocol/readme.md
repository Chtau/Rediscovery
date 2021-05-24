# P2P Rediscovery Protocol

## Discovery

The protocol has a permanet discovery in the background for new devices.
Devices will be added to an internal device collection and via callback exposed.
Handshake provides the settings for data and low data receive port.
After one discovery listener receives handshake setting from a peer it sends it's own handshake setting.


```json
{
    "Identifier": "53452134324234234234@10C4078FF442434CAE4816D89018F516@DeviceA",
    "FriendlyName": "Device A",
    "Data": {
        "Port": 13575,
        "Size": 1024
    },
    "LowData": {
        "Port": 13578,
        "Size": 1024
    },
    "Metadata": {
        "OS": "Android",
        "Idiom": "Phone",
        "ScreenSize": {
            "Width": 1920,
            "Height": 1080
        },
        "User": "Test A",
        "Machine": "Device A",
        "Is64Bit": true,
        "Processor": 8,
        "PhysicalMemory": 64000
    }
}
```

`Identifier` is a combination of values to create a unique key per device. The values use a `@` as delimiter.

1. Ticks as at the time of first creating the identifier
2. Guid without delimiter
3. Device name

`FriendlyName` is a user defined name for the device. (Can be inherited from the OS)


## Data Transfer
Default data transfer between peer based on TCP to make sure packages are delivered and received.


## Low Data Transfer
A low latency unsafe way of transfering data between peers based on UDP. (Packages can be lost) This should mainly be used for low latency required data transfers like real time streams.