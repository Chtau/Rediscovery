import { Injectable, NgZone, EventEmitter } from "@angular/core";
import { ILoggerEntry, LoggerType } from "./logger.interface";
import { environment } from "src/environments/environment";

import * as dummyLoggerEntries from '../../assets/dummy/logger.json';
import { IpcService } from "../ipc.service";

@Injectable()
export class LoggerService {

  entriesChanged = new EventEmitter<ILoggerEntry[]>();
  entryAddedChanged = new EventEmitter<ILoggerEntry>();

  entries: ILoggerEntry[] = [];
  
  constructor(private ipc: IpcService,private zone: NgZone) {
  }

  public initIPC(): void {
    console.log("init Logger IPC");
    if (environment.isElectron === false) {
      this.entries = <ILoggerEntry[]>dummyLoggerEntries.default;
    } else {
      this.ipc.on('loggermessage-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.entries.push(arg);
          this.entriesChanged.emit(this.entries);
          this.entryAddedChanged.emit(arg);
        });
      });
    }
  }

  public getEntries(): ILoggerEntry[] {
    return this.entries;
  }

}
