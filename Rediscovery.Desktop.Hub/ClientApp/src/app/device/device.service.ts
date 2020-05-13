import { Injectable, EventEmitter, NgZone } from "@angular/core";
import { environment } from "src/environments/environment";

//const { ipcRenderer } = window.require("electron");
//import { ipcRenderer } from "electron";


import * as dummyDevice from '../../assets/dummy/device.json';
import * as dummyConnectedDevice from '../../assets/dummy/connecteddevice.json';
import * as dummyPendingDevice from '../../assets/dummy/pendingdevice.json';
import { IpcService } from "../ipc.service";

@Injectable()
export class DeviceService {

  registeredDevicesChanged = new EventEmitter<IDeviceInfo[]>();
  connectedDevicesChanged = new EventEmitter<IDeviceInfo[]>();
  pendingDevicesChanged = new EventEmitter<IDeviceInfo[]>();

  registeredDeviceModels: IDeviceInfo[] = [];
  connectedDeviceModels: IDeviceInfo[] = [];
  pendingDeviceModels: IDeviceInfo[] = [];
  
  constructor(private ipc: IpcService,private zone: NgZone) {
    
  }

  public initIPC(): void {
    console.log("init Device IPC");
    if (environment.isElectron === false) {
      this.registeredDeviceModels = <IDeviceInfo[]>dummyDevice.default;
      this.connectedDeviceModels = <IDeviceInfo[]>dummyConnectedDevice.default;
      this.pendingDeviceModels = <IDeviceInfo[]>dummyPendingDevice.default;
    } else {
      this.ipc.on('registereddeviceinfo-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.registeredDeviceModels = arg;
          this.registeredDevicesChanged.emit(this.registeredDeviceModels);
        });
      });
      this.ipc.on('activedeviceinfo-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.connectedDeviceModels = arg;
          this.connectedDevicesChanged.emit(this.connectedDeviceModels);
        });
      });
      this.ipc.on('pendingdevice-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.pendingDeviceModels = arg;
          this.pendingDevicesChanged.emit(this.pendingDeviceModels);
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

  public getPendingDevices(): IDeviceInfo[] {
    return this.pendingDeviceModels;
  }

  public getDeviceDetail(id: string): IDeviceInfo {
    return this.registeredDeviceModels.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }

  public getConnectedDeviceDetail(id: string): IDeviceInfo {
    return this.connectedDeviceModels.find(x => {
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

  public getPendingDeviceDetail(id: string): IDeviceInfo {
    return this.pendingDeviceModels.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }

  public resolvePendingDevice(id: string, accept: boolean): void {
    
  }
}
