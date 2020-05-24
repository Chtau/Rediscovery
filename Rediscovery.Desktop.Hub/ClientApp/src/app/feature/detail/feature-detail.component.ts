import { Component, AfterViewInit } from '@angular/core';
import { FeatureService } from '../feature.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { IOService } from 'src/app/io.service';

@Component({
  selector: 'app-feature-detail',
  templateUrl: './feature-detail.component.html',
  styleUrls: ['./feature-detail.component.css']
})
export class FeatureDetailComponent implements AfterViewInit {
  
  model: IDeviceFeature = null;
  settingsUrl: string = null;

  constructor(private featureService: FeatureService,
    private ioService: IOService,
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
    this.model = this.featureService.getFeatureDetail(id);
    /*this.featureService.getFeatureDetailUI(id).subscribe(result => {
      this.settingsUrl = result;
    });*/
    this.featureService.featuresProfileUIReceived.subscribe((result: IEntityContent) => {
      console.log('received Profile UI:' + JSON.stringify(result));
      if (result.id == this.model.id) {
        this.settingsUrl = result.content;
      }
    });
    this.featureService.requestFeatureDetailUI(id);
  }

  openFolder(): void {
    this.ioService.openDirectory(this.model.pluginDirectory);
  }
}
