import { Component, ViewChild, ElementRef, OnInit, AfterViewChecked, AfterViewInit } from '@angular/core';
import { LoggerService } from './logger.service';
import { ILoggerEntry, LoggerType } from './logger.interface';

@Component({
  selector: 'app-logger',
  templateUrl: './logger.component.html',
  styleUrls: ['./logger.component.css']
})
export class LoggerComponent implements OnInit, AfterViewChecked, AfterViewInit {
  @ViewChild('terminalContainer', {static: true}) private terminalContainer: ElementRef;

  autoscroll: boolean = true;
  entries: ILoggerEntry[] = [];
  sourceEntries: ILoggerEntry[] = [];

  showTrace: boolean = true;
  showDebug: boolean = true;
  showInformation: boolean = true;
  showWarning: boolean = true;
  showError: boolean = true;
  showCritical: boolean = true;

  constructor(private loggerService: LoggerService) {
    this.sourceEntries = loggerService.getEntries();
    this.setEntires();
    this.scrollToBottom();
    this.loggerService.entryAddedChanged.subscribe(result => {
      this.sourceEntries.push(result);
      this.setEntires();
      this.scrollToBottom();
    });
  }

  ngOnInit() { 
    this.scrollToBottom();
  }

  ngAfterViewChecked() {
      // Called every time the view changes        
      this.scrollToBottom();        
  } 

  ngAfterViewInit() {
      // Only called ONCE => upon initialization
      this.scrollToBottom();
  }


  scrollToBottom(): void {
    try {
      if (this.autoscroll === true) {
        this.terminalContainer.nativeElement.scrollTop = this.terminalContainer.nativeElement.scrollHeight;
      }
    } catch(err) { }                 
  }

  clearConsole(): void {
    this.entries = [];
    this.sourceEntries = [];
  }

  changeAutoscroll(): void {
    this.autoscroll = !this.autoscroll;
  }

  filterTrace():void {
    this.showTrace = !this.showTrace;
    this.setEntires();
  }

  filterDebug():void {
    this.showDebug = !this.showDebug;
    this.setEntires();
  }
  
  filterInformation():void {
    this.showInformation = !this.showInformation;
    this.setEntires();
  }
    
  filterWarning():void {
    this.showWarning = !this.showWarning;
    this.setEntires();
  }
      
  filterError():void {
    this.showError = !this.showError;
    this.setEntires();
  }
        
  filterCritical():void {
    this.showCritical = !this.showCritical;
    this.setEntires();
  }

  private setEntires(): void {
    this.entries = this.sourceEntries.filter(item => {
      if (this.showTrace == true && item.type == LoggerType.Trace) {
        return true;
      }
      if (this.showDebug == true && item.type == LoggerType.Debug) {
        return true;
      }
      if (this.showInformation == true && item.type == LoggerType.Information) {
        return true;
      }
      if (this.showWarning == true && item.type == LoggerType.Warning) {
        return true;
      }
      if (this.showError == true && item.type == LoggerType.Error) {
        return true;
      }
      if (this.showCritical == true && item.type == LoggerType.Critical) {
        return true;
      }
    });
  }
}
