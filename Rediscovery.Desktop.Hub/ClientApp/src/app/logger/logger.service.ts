import { Injectable } from "@angular/core";
import { ILoggerEntry, LoggerType } from "./logger.interface";
import { environment } from "src/environments/environment";

@Injectable()
export class LoggerService {
  models: ILoggerEntry[] = [];
  
  constructor() {
    if (environment.isElectron === false) {
      this.models = [
        {
          id: "0",
          text: "Test",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "1",
          text: "Test 1",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "2",
          text: "Test 2",
          subText: "Additional Text",
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "3",
          text: "Test Warning",
          subText: null,
          time: new Date(),
          type: LoggerType.Warning
        },
        {
          id: "4",
          text: "Test Error",
          subText: null,
          time: new Date(),
          type: LoggerType.Error
        },
        {
          id: "0",
          text: "Test",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "1",
          text: "Test 1",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "2",
          text: "Test 2",
          subText: "Additional Text",
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "3",
          text: "Test Warning",
          subText: null,
          time: new Date(),
          type: LoggerType.Warning
        },
        {
          id: "4",
          text: "Test Error",
          subText: null,
          time: new Date(),
          type: LoggerType.Error
        },
        {
          id: "0",
          text: "Test",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "1",
          text: "Test 1",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "2",
          text: "Test 2",
          subText: "Additional Text",
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "3",
          text: "Test Warning",
          subText: null,
          time: new Date(),
          type: LoggerType.Warning
        },
        {
          id: "4",
          text: "Test Error",
          subText: null,
          time: new Date(),
          type: LoggerType.Error
        },
        {
          id: "0",
          text: "Test",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "1",
          text: "Test 1",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "2",
          text: "Test 2",
          subText: "Additional Text",
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "3",
          text: "Test Warning",
          subText: null,
          time: new Date(),
          type: LoggerType.Warning
        },
        {
          id: "4",
          text: "Test Error",
          subText: null,
          time: new Date(),
          type: LoggerType.Error
        },
        {
          id: "0",
          text: "Test",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "1",
          text: "Test 1",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "2",
          text: "Test 2",
          subText: "Additional Text",
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "3",
          text: "Test Warning",
          subText: null,
          time: new Date(),
          type: LoggerType.Warning
        },
        {
          id: "4",
          text: "Test Error",
          subText: null,
          time: new Date(),
          type: LoggerType.Error
        },
        {
          id: "0",
          text: "Test",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "1",
          text: "Test 1",
          subText: null,
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "2",
          text: "Test 2",
          subText: "Additional Text",
          time: new Date(),
          type: LoggerType.Normal
        },
        {
          id: "3",
          text: "Test Warning",
          subText: null,
          time: new Date(),
          type: LoggerType.Warning
        },
        {
          id: "4",
          text: "Test Error",
          subText: null,
          time: new Date(),
          type: LoggerType.Error
        }
      ];
    }
  }

  public getEntries(): ILoggerEntry[] {
    return this.models;
  }

}
