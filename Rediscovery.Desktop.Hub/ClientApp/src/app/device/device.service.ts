import { Injectable } from "@angular/core";

@Injectable()
export class DeviceService {

  models: IDeviceInfo[] = [
    {
      name: "Test 1",
      id: "0",
      allowAccess: true
    },
    {
      name: "Test 2",
      id: "1",
      allowAccess: true
    }
  ];
  
  constructor() {

  }

  public getRegisteredDevices(): IDeviceInfo[] {
    return this.models;
  }

  public getConnectedDevices(): IDeviceInfo[] {
    return this.models;
  }

  public getDeviceDetail(id: string): IDeviceInfo {
    return this.models.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }

  public getRegisteredDeviceDetail(id: string): IDeviceInfo {
    return this.models.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }
}
