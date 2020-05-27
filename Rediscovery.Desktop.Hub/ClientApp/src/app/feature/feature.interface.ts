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
  featureId: string;
  data: string;
}

interface IDeviceFeatureProfil {
  id: string;
  featureId: string;
  displayName: string;
  profileData: string;
}