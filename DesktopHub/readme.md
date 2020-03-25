## TODO: We should switch to Electron.Net for Desktop UI Plugin Support

## Publish for Windows

dotnet publish --configuration Release -r win-x64 --self-contained true


## Command Line arguments

### Show service info window

This window requires the argument 

--serviceinfo:192.168.1.100

### Show incomming connection Authentication window

This window requires the arguments

--code:123456

--device:dev

--valid:123124124
(value are date ticks)

