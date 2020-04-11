import { Component, Inject, ChangeDetectorRef } from '@angular/core';
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
    this.connectedDeviceModels = this.deviceService.getConnectedDevices();
    this.registeredDeviceModels = this.deviceService.getRegisteredDevices();

    this.deviceService.registeredDevicesChanged.subscribe(result => {
      this.registeredDeviceModels = result;
    });
  }

}
