import { Injectable, NgZone, EventEmitter } from "@angular/core";
import { environment } from "src/environments/environment";
import { IpcService } from "./ipc.service";

@Injectable()
export class StateService {

  serviceConnectionStateChanged = new EventEmitter<boolean>();

  serviceConnectionState: boolean = false;
  
  constructor(private ipc: IpcService,private zone: NgZone) {
  }

  public initIPC(): void {
    console.log("init State IPC");
    if (environment.isElectron === false) {
      
    } else {
      this.ipc.on('hubconnectionchanged-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.serviceConnectionState = arg;
          this.serviceConnectionStateChanged.emit(this.serviceConnectionState);
        });
      });
    }
  }

  public getServiceConnectionState(): boolean {
    return this.serviceConnectionState;
  }

}
