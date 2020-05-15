import { Component, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { DeviceService } from '../device.service';

@Component({
  selector: 'app-device-registered',
  templateUrl: './device-registered.component.html',
  styleUrls: ['./device-registered.component.css']
})
export class DeviceRegisteredComponent implements AfterViewInit {
  
  model: IDeviceInfo = null;
  allowAccessCheck: boolean = true;
  saveDisabled: boolean = true;
  confirmModal: boolean = false;

  constructor(private deviceService: DeviceService,
    private route: ActivatedRoute,
    private router: Router) {
    this.onLoadModel(this.route.snapshot.paramMap.get('id'));
    router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.onLoadModel(this.route.snapshot.paramMap.get('id'));
      }
    });
  }

  ngAfterViewInit(): void {
    
  }

  private onLoadModel(id: string): void {
    this.model = this.deviceService.getRegisteredDeviceDetail(id);
    this.allowAccessCheck = this.model.allowAccess;
  }

  onAllowAccessChanged(event): void {
    if (this.allowAccessCheck === this.model.allowAccess) {
      this.saveDisabled = false;
    } else {
      this.saveDisabled = true;
    }
  }

  onSave(): void {
    this.model.allowAccess = this.allowAccessCheck;
    this.deviceService.updateDevice(this.model);
  }

  onDelete(): void {
    this.confirmModal = true;
  }

  onModalConfirm(): void {
    this.deviceService.deleteDevice(this.model);
  }

}
