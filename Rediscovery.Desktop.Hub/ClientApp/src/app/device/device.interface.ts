interface IDeviceInfo {
  name: string;
  id: string;
  allowAccess: boolean;
}

interface IPendingAuthenticationResolve {
  id: string;
  accept: boolean;
}