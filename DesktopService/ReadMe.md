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


### IPC

#### Provider

|Hub|Type|Info|
|-----|-----|----|
|`rediscoveryhub`|`IncomingConnectionInfo`|Data for a new incoming connection|
|`rediscoveryhublivelogger`|`LoggerEntryModel`|Logger data|
|`rediscoveryservice`|`PipeResource<T>`|Provide requested Resource pipe data object|

#### Receiver

|Hub|Type|Info|
|-----|-----|----|
|`rediscoveryservice`|`string`|Expect `deviceinfo` or `features` to provide the requested resource type|
|`sync_device_rediscoveryservice`|`IPCPipe.Models.Sync<SharedCoreModels.DeviceInfo>`|Handle model changes from an IPC Client|


## Authentication Flow

The Client invokes the `Welcome` function on the Service to start the Authentication.
If the Client is already authenticated or the Service has the Authentication deactivated the Service will invoke the Client function `Hello`with a new `Bearer` Token, after this the Service invokes the `Manifest` Client function to complete the Authentication.

When the Client is unknown to the Service and required authentication the Service will respond after the `Welcome` with the call of the Client `Hello` function for further Client actions.
* `WaitForApprovel` Service creates a short Key which the Client is required to send with the function `AuthorizeKey`
* `Denied` Service denied access
* `Failed` Internal Service error
