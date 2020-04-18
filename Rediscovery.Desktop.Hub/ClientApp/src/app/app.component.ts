import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DeviceService } from './device/device.service';
import { LoggerService } from './logger/logger.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
  private deviceService: DeviceService,
  private loggerService: LoggerService) {
    this.deviceService.initIPC();
    this.loggerService.initIPC();
    http.get<boolean>(baseUrl + 'communication').subscribe(result => {
      console.log('Communication to Service init');
    }, error => console.error(error));
  }

  private onExit():void {
    
  }

  onServiceConnection(): void {
    this.http.get<boolean>(this.baseUrl + 'communication').subscribe(result => {
      console.log('Communication to Service init');
    }, error => console.error(error));
  }

}
