import { Injectable, EventEmitter, NgZone } from "@angular/core";
import { environment } from "src/environments/environment";

import { IpcService } from "./ipc.service";
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import {map} from 'rxjs/operators';

@Injectable()
export class IOService {

  constructor(private ipc: IpcService,private zone: NgZone) {
    
  }

  public initIPC(): void {
    console.log("init IO IPC");
    if (environment.isElectron === false) {
    } else {
      
    }
  }

  public openDirectory(directory: string): void {
    this.ipc.send("open-directory", directory);
  }
}
