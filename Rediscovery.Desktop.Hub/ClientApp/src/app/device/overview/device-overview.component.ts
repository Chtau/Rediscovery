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
  pendingDeviceModels: IDeviceInfo[] = [];
  selectedPendingDevice: IDeviceInfo[] = [];

  constructor(private deviceService: DeviceService) {
    this.connectedDeviceModels = deviceService.getConnectedDevices();
    this.registeredDeviceModels = deviceService.getRegisteredDevices();
    this.pendingDeviceModels = deviceService.getPendingDevices();

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

  onAcceptSelectedPendingDevices():void {
    this.onResolvePendingDevices(true);
  }

  onRemoveSelectedPendingDevices():void {
    this.onResolvePendingDevices(true);
  }

  private onResolvePendingDevices(acceptValue: boolean):void {
    if (this.selectedPendingDevice.length > 0) {
      this.selectedPendingDevice.forEach(item => {
        this.deviceService.resolvePendingDevice({
          id: item.id,
          accept: acceptValue
        });
      });
    }
  }

}
