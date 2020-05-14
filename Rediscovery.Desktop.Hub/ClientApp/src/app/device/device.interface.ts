interface IDeviceInfo {
  name: string;
  id: string;
  identifier: string;
  allowAccess: boolean;
  requestTime?: Date;
  model: string;
  manufacturer: string;
  oSVersion: string;
  platform: string;
  idiom: string;
  deviceType: string;
}

interface IPendingAuthenticationResolve {
  id: string;
  accept: boolean;
}