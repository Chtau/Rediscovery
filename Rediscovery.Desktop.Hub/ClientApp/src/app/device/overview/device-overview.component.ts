import { Component } from '@angular/core';
import { DeviceService } from '../device.service';

@Component({
  selector: 'app-device-overview',
  templateUrl: './device-overview.component.html',
  styleUrls: ['./device-overview.component.css']
})
export class DeviceOverviewComponent {
  
  connectedDeviceModels: IDeviceInfo[] = [];
  registeredDeviceModels: IDeviceInfo[] = [];

  constructor(private deviceService: DeviceService) {
    this.connectedDeviceModels = deviceService.getConnectedDevices();
    this.registeredDeviceModels = deviceService.getRegisteredDevices();

    this.deviceService.registeredDevicesChanged.subscribe(result => {
      this.registeredDeviceModels = result;
    });
  }

}
