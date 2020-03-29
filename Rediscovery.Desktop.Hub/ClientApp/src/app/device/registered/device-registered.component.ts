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
  }

}
