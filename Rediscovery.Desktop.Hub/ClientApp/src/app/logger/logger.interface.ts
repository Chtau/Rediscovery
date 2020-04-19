export enum LoggerType {
  Trace = 0,
  Debug = 1,
  Information = 2,
  Warning = 3,
  Error = 4,
  Critical = 5
};

export interface ILoggerEntry {
  id: string;
  text: string;
  subText?: string;
  time: Date;
  type: LoggerType;
};