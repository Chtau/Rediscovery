import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

//const { ipcRenderer } = window.require("electron");
//import { ipcRenderer } from "electron";


import * as dummyDevice from '../../assets/dummy/device.json';
import { IpcService } from "../ipc.service";

@Injectable()
export class DeviceService {

  models: IDeviceInfo[] = [];
  
  constructor(private ipc: IpcService) {
    if (environment.isElectron === false) {
      this.models = <IDeviceInfo[]>dummyDevice.default;
    } else {
      ipc.on('asynchronous-reply', (event, arg) => {
        console.log(arg);
        this.models = arg;
      });
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
