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


  entries: ILoggerEntry[] = [];

  constructor(private loggerService: LoggerService) {
    this.entries = loggerService.getEntries();
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
        this.terminalContainer.nativeElement.scrollTop = this.terminalContainer.nativeElement.scrollHeight;
    } catch(err) { }                 
  }

}
