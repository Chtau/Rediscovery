# Rediscovery Service

## API


### Feature (required authenticated User)

#### Stateless (HTML Endpoints)
|Route|Type|Info|
|-----|----|----|
|`features/ui/{featureId}`|`FileContentResult`|Zip Archive (ui.zip) containing the Feature GUI or not found if the Feature id is invalid|
|`features/profiles/{featureId}`|`List<DeviceFeatureProfil>`|Collection of all Profiles for this Feature|
|`features/settings/{featureId}`|`DeviceFeatureSetting`|Settings for this Feature|

#### Real time communication to the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`ClientMessage(Guid featureId, string profileId, object data)`|The received data will be redirected to the Feature|
|`ClientFeatureStart(Guid featureId)`|The Client is required to invoke this function before the feature starts the real time communication|
|`ClientFeatureStop(Guid featureId)`|When invoked the feature will stop all real time communication|

#### Real time communication from the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`ClientResponse(Guid featureId, object data)`|The Service invokes this Client function with the data provided by the Feature|


### Authentication

#### Real time communication to the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`Welcome(string device)`|Starts the authorization flow for a Device|
|`AuthorizeKey(string device, string key)`|Authenticates the Device with a key|

#### Real time communication from the Service (SignalR Hub)

|Function|Info|
|-----|----|
|`Manifest(Manifest manifest)`|This function will be invoked to provide the Manifest data from the service|
|`Hello(ConnectionState connectionState, string token)`|This function will be invoked to inform the Client about the Authentication state and `Bearer` Token if successful|



