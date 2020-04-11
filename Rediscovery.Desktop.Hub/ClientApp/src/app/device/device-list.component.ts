import { Component, Inject, ChangeDetectorRef } from '@angular/core';
import { DeviceService } from './device.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css']
})
export class DeviceListComponent {
  
  connectedDeviceModels: IDeviceInfo[] = [];
  registeredDeviceModels: IDeviceInfo[] = [];

  constructor(private deviceService: DeviceService,
    http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.connectedDeviceModels = this.deviceService.getConnectedDevices();
    this.registeredDeviceModels = this.deviceService.getRegisteredDevices();

    http.get<boolean>(baseUrl + 'device').subscribe(result => {
      //this.forecasts = result;
      console.log('device list call');
    }, error => console.error(error));

    this.deviceService.registeredDevicesChanged.subscribe(result => {
      this.registeredDeviceModels = result;
    });
  }

}
