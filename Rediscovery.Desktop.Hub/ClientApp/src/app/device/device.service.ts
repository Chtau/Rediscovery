import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

import * as dummyDevice from '../../assets/dummy/device.json';

@Injectable()
export class DeviceService {

  models: IDeviceInfo[] = [];
  
  constructor() {
    if (environment.isElectron === false) {
      this.models = <IDeviceInfo[]>dummyDevice.default;
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
