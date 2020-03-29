export enum LoggerType {
  Normal = 0,
  Error = 1,
  Warning = 2
};

export interface ILoggerEntry {
  id: string;
  text: string;
  subText?: string;
  time: Date;
  type: LoggerType;
};