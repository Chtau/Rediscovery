import { Component } from '@angular/core';
import { DeviceService } from '../device.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-device-overview',
  templateUrl: './device-overview.component.html',
  styleUrls: ['./device-overview.component.css']
})
export class DeviceOverviewComponent {
  
  connectedDeviceModels: IDeviceInfo[] = [];
  registeredDeviceModels: IDeviceInfo[] = [];

  constructor(private deviceService: DeviceService, private route: Router) {
    this.connectedDeviceModels = deviceService.getConnectedDevices();
    this.registeredDeviceModels = deviceService.getRegisteredDevices();

    this.deviceService.registeredDevicesChanged.subscribe(result => {
      this.registeredDeviceModels = result;
    });

    this.deviceService.connectedDevicesChanged.subscribe(result => {
      this.connectedDeviceModels = result;
    });
  }

  onEditConnected(model: IDeviceInfo): void {
    this.route.navigate(['/devices/',model.id])
  }

  onEditRegistered(model: IDeviceInfo): void {
    this.route.navigate(['/devices/registered/',model.id])
  }
}
