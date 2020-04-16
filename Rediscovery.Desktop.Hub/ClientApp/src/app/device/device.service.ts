import { Injectable, EventEmitter, NgZone } from "@angular/core";
import { environment } from "src/environments/environment";

//const { ipcRenderer } = window.require("electron");
//import { ipcRenderer } from "electron";


import * as dummyDevice from '../../assets/dummy/device.json';
import { IpcService } from "../ipc.service";

@Injectable()
export class DeviceService {

  registeredDevicesChanged = new EventEmitter<IDeviceInfo[]>();
  connectedDevicesChanged = new EventEmitter<IDeviceInfo[]>();

  registeredDeviceModels: IDeviceInfo[] = [];
  connectedDeviceModels: IDeviceInfo[] = [];
  
  constructor(private ipc: IpcService,private zone: NgZone) {
    
  }

  public initIPC(): void {
    console.log("init Device IPC");
    if (environment.isElectron === false) {
      this.registeredDeviceModels = <IDeviceInfo[]>dummyDevice.default;
    } else {
      this.ipc.on('registereddeviceinfo-ipc', (event, arg) => {
        console.log("IPC data received");
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.registeredDeviceModels = arg;
          this.registeredDevicesChanged.emit(this.registeredDeviceModels);
        });
      });
    }
  }

  public getRegisteredDevices(): IDeviceInfo[] {
    return this.registeredDeviceModels;
  }

  public getConnectedDevices(): IDeviceInfo[] {
    return this.connectedDeviceModels;
  }

  public getDeviceDetail(id: string): IDeviceInfo {
    return this.registeredDeviceModels.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }

  public getRegisteredDeviceDetail(id: string): IDeviceInfo {
    return this.registeredDeviceModels.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }
}
