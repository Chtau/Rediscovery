import { Injectable } from "@angular/core";
import { ILoggerEntry, LoggerType } from "./logger.interface";
import { environment } from "src/environments/environment";

import * as dummyLoggerEntries from '../../../assets/dummy/logger.json';

@Injectable()
export class LoggerService {
  models: ILoggerEntry[] = [];
  
  constructor() {
    if (environment.isElectron === false) {
      this.models = <ILoggerEntry[]>dummyLoggerEntries.default;
    }
  }

  public getEntries(): ILoggerEntry[] {
    return this.models;
  }

}
