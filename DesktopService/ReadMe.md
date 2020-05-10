# Rediscovery Service

## API


### Feature (required authenticated User)

#### Stateless (HTML Endpoints)
|Route|Type|Info|
|-----|----|----|
|`features/ui/{featureId}`|`FileContentResult`|Zip Archive (ui.zip) containing the Feature GUI or not found if the Feature id is invalid|
|`features/profiles/{featureId}`|`List<DeviceFeatureProfil>`|Collection of all Profiles for this Feature|
|`features/settings/{featureId}`|`DeviceFeatureSetting`|Settings for this Feature|

#### SignalR Hub Endpoint `/hubs/feature`

##### Real time communication to the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`ClientMessage(Guid featureId, string profileId, object data)`|The received data will be redirected to the Feature|
|`ClientFeatureStart(Guid featureId)`|The Client is required to invoke this function before the feature starts the real time communication|
|`ClientFeatureStop(Guid featureId)`|When invoked the feature will stop all real time communication|

##### Real time communication from the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`ClientResponse(Guid featureId, object data)`|The Service invokes this Client function with the data provided by the Feature|


### Authentication

#### SignalR Hub Endpoint `/hubs/connect`

##### Real time communication to the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`Welcome(string device)`|Starts the authorization flow for a Device|
|`AuthorizeKey(string device, string key)`|Authenticates the Device with a key|

##### Real time communication from the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`Manifest(Manifest manifest)`|This function will be invoked to provide the Manifest data from the service|
|`Hello(ConnectionState connectionState, string token)`|This function will be invoked to inform the Client about the Authentication state and `Bearer` Token if successful|


### Remote resource

#### SignalR Hub Endpoint `/remote/resource/hub`

Do receive data the Client has to call `Hello` with the correct application key.


##### Receiver

|Function|Info|
|-----|----|
|`Hello(string applicationKey)`|Adds the device to the Group which can receive resources|
|`RequestActiveDeviceInfo()`|Request all active devices connected to the service|
|`RequestDeviceInfo()`|Request all known devices to the service|
|`RequestServiceFeature()`|Request all supported service features|
|`RequestDeleteDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo)`|Request deletion of a device|

###### Sender

|Function|Info|
|-----|----|
|`Hello(string result)`|Sends a hello response result with a valid authentication `Bearer` token (if valid application key) or `null` (application key is unknown to the service)|
|`ActiveDeviceInfo(List<SharedCoreModels.DeviceInfo> deviceInfos)`|Sends all active devices which are connected to the service|
|`DeviceInfo(List<SharedCoreModels.DeviceInfo> deviceInfos)`|Sends all devices known by the service|
|`ServiceFeature(List<SharedCoreModels.DeviceFeature> deviceInfos)`|Sends all supported feature by the service|
|`LogEntry(LoggerEntryModel loggerModel)`|Sends new logger entry|


#### SignalR Hub Endpoint `/remote/resource/discovery`

Do receive data the Client has to call `Hello` with the correct application key.


##### Receiver

|Function|Info|
|-----|----|
|`Hello(string applicationKey)`|Adds the device to the Group which can receive resources|

###### Sender

|Function|Info|
|-----|----|
|`Hello(string result)`|Sends a hello response result with a valid authentication `Bearer` token (if valid application key) or `null` (application key is unknown to the service)|

#### SignalR Hub Endpoint `/remote/resource/info`

Do receive data the Client has to call `Hello` with the correct application key.


##### Receiver

|Function|Info|
|-----|----|
|`Hello(string applicationKey)`|Adds the device to the Group which can receive resources|

###### Sender

|Function|Info|
|-----|----|
|`Hello(string result)`|Sends a hello response result with a valid authentication `Bearer` token (if valid application key) or `null` (application key is unknown to the service)|
|`NewValidationCode(SharedCoreModels.IncomingConnectionInfo connectionInfo)`|Sends connection validation data|
|`ApplicationInfo(string serviceInfo)`|Sends service info|


## Authentication Flow

The Client invokes the `Welcome` function on the Service to start the Authentication.
If the Client is already authenticated or the Service has the Authentication deactivated the Service will invoke the Client function `Hello`with a new `Bearer` Token, after this the Service invokes the `Manifest` Client function to complete the Authentication.

When the Client is unknown to the Service and required authentication the Service will respond after the `Welcome` with the call of the Client `Hello` function for further Client actions.
* `WaitForApprovel` Service creates a short Key which the Client is required to send with the function `AuthorizeKey`
* `Denied` Service denied access
* `Failed` Internal Service error


## Plugins

### Feature Plugins

Feature Plugins must implement the Interface `IDeviceFeatureImplementation` in a public class.
When the Plugin Assembly is loaded the `Init` function will be called to provide the Directory from which the Plugin is loaded and additional Interfaces which the Plugin can use.

The Project file of the Plugin required a manual change to add the following Item group.
This change will make sure that the Plugin dosen't copy the `PluginBase.dll` to the Output. (For more Info [Mirosoft Docs](https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support))

```xml
<ItemGroup>
    <ProjectReference Include="..\PluginBase\PluginBase.csproj">
        <Private>false</Private>
        <ExcludeAssets>runtime</ExcludeAssets>
    </ProjectReference>
</ItemGroup>
```
