import { Component, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { DeviceService } from '../device.service';

@Component({
  selector: 'app-device-pending',
  templateUrl: './device-pending.component.html',
  styleUrls: ['./device-pending.component.css']
})
export class DevicePendingComponent implements AfterViewInit {
  
  model: IDeviceInfo = null;

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
    this.model = this.deviceService.getPendingDeviceDetail(id);
  }

}
