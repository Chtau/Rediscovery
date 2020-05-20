import { Component } from '@angular/core';
import { DeviceService } from '../device/device.service';

@Component({
  selector: 'app-pending-authentication',
  templateUrl: './pending-authentication.component.html',
  styleUrls: ['./pending-authentication.component.css']
})
export class PendingAuthenticationComponent {
  
  pendingDeviceModels: IDeviceInfo[] = [];
  selectedPendingDevice: IDeviceInfo[] = [];

  constructor(private deviceService: DeviceService) {
    this.pendingDeviceModels = deviceService.getPendingDevices();
    this.deviceService.pendingDevicesChanged.subscribe(result => {
      this.pendingDeviceModels = result;
    });
  }

  onAcceptSelectedPendingDevices():void {
    this.onResolvePendingDevices(true);
  }

  onRemoveSelectedPendingDevices():void {
    this.onResolvePendingDevices(false);
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
