# Rediscovery Service

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
