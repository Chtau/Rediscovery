import { Component } from '@angular/core';
import { FeatureService } from '../feature.service';

@Component({
  selector: 'app-feature-list-overview',
  templateUrl: './feature-list-overview.component.html',
  styleUrls: ['./feature-list-overview.component.css']
})
export class FeatureListOverviewComponent {
  
  public featureModels: IDeviceFeature[] = [];

  constructor(private featureService: FeatureService) {
    this.featureModels = featureService.getFeatures();

    this.featureService.featuresChanged.subscribe(result => {
      this.featureModels = result;
    });
  }

}
