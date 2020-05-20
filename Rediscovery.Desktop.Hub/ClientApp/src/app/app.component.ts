import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DeviceService } from './device/device.service';
import { LoggerService } from './logger/logger.service';
import { FeatureService } from './feature/feature.service';
import { environment } from 'src/environments/environment';
import { StateService } from './state.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  
  isServiceConnected: boolean = false;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
  private deviceService: DeviceService,
  private loggerService: LoggerService,
  private featuerService: FeatureService,
  private stateService: StateService) {
    this.deviceService.initIPC();
    this.loggerService.initIPC();
    this.featuerService.initIPC();
    this.stateService.initIPC();
    this.stateService.serviceConnectionStateChanged.subscribe(result => {
      this.isServiceConnected = result;
    });
    this.onServiceConnection();
  }

  private onExit():void {
    
  }

  onServiceConnection(): void {
    if (environment.isElectron === true) {
      this.http.get<boolean>(this.baseUrl + 'communication').subscribe(result => {
        console.log('Communication to Service init');
        this.isServiceConnected = this.stateService.getServiceConnectionState();
      }, error => console.error(error));
    }
  }

}
