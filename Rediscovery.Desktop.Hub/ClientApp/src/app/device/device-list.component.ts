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
  pendingDeviceModels: IDeviceInfo[] = [];

  constructor(private deviceService: DeviceService) {
    this.connectedDeviceModels = this.deviceService.getConnectedDevices();
    this.registeredDeviceModels = this.deviceService.getRegisteredDevices();
    this.pendingDeviceModels = this.deviceService.getPendingDevices();

    this.deviceService.registeredDevicesChanged.subscribe(result => {
      this.registeredDeviceModels = result;
    });

    this.deviceService.connectedDevicesChanged.subscribe(result => {
      this.connectedDeviceModels = result;
    });

    this.deviceService.pendingDevicesChanged.subscribe(result => {
      this.pendingDeviceModels = result;
    });
  }

}
