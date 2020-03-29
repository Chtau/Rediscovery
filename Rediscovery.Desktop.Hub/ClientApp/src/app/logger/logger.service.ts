import { Injectable } from "@angular/core";
import { ILoggerEntry, LoggerType } from "./logger.interface";

@Injectable()
export class LoggerService {
  models: ILoggerEntry[] = [
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
  
  constructor() {

  }

  public getEntries(): ILoggerEntry[] {
    return this.models;
  }

}
