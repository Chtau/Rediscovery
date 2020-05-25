interface IDeviceFeature {
  id: string;
  displayName: string;
  version: string;
  minFeatureIntegrationPoint: string;
  minControlIntegrationPoint: string;
  hasSettings: boolean;
  hasProfiles: boolean;
  author: string;
  documentation: string;
  url: string;
  pluginDirectory: string;
}

interface IDeviceFeatureSetting {
  data: any;
}

interface IDeviceFeatureProfil {
  id: string;
  displayName: string;
  profileData: string;
}