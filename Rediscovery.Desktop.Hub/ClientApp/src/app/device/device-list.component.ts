import { Component } from '@angular/core';
import { DeviceService } from './device.service';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css']
})
export class DeviceListComponent {
  
  connectedDeviceModels: IDeviceInfo[] = [];
  registeredDeviceModels: IDeviceInfo[] = [];

  constructor(private deviceService: DeviceService) {
    this.connectedDeviceModels = deviceService.getConnectedDevices();
    this.registeredDeviceModels = deviceService.getRegisteredDevices();
  }

}
