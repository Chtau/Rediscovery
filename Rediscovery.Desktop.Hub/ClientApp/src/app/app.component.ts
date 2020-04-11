import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DeviceService } from './device/device.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string,
  private deviceService: DeviceService) {
    this.deviceService.initIPC();
    http.get<boolean>(baseUrl + 'communication').subscribe(result => {
      console.log('Communication to Service init');
    }, error => console.error(error));
  }

  private onExit():void {
    
  }

}
