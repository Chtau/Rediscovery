import { Component, ViewChild, ElementRef, OnInit, AfterViewChecked, AfterViewInit } from '@angular/core';
import { LoggerService } from './logger.service';
import { ILoggerEntry } from './logger.interface';

@Component({
  selector: 'app-logger',
  templateUrl: './logger.component.html',
  styleUrls: ['./logger.component.css']
})
export class LoggerComponent implements OnInit, AfterViewChecked, AfterViewInit {
  @ViewChild('terminalContainer', {static: true}) private terminalContainer: ElementRef;

  autoscroll: boolean = true;
  entries: ILoggerEntry[] = [];

  constructor(private loggerService: LoggerService) {
    this.entries = loggerService.getEntries();
    this.scrollToBottom();
    this.loggerService.entryAddedChanged.subscribe(result => {
      this.entries.push(result);
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
  }

  changeAutoscroll(): void {
    this.autoscroll = !this.autoscroll;
  }

}
