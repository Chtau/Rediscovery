import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable()
export class DeviceService {

  models: IDeviceInfo[] = [];
  
  constructor() {
    if (environment.isElectron === false) {
      this.models = [
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
    }
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
