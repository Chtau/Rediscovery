import { Component } from '@angular/core';
import { FeatureService } from './feature.service';

@Component({
  selector: 'app-feature-list',
  templateUrl: './feature-list.component.html',
  styleUrls: ['./feature-list.component.css']
})
export class FeatureListComponent {
  
  public featureModels: IDeviceFeature[] = [];

  constructor(private featureService: FeatureService) {
    this.featureModels = featureService.getFeatures();

    this.featureService.featuresChanged.subscribe(result => {
      this.featureModels = result;
    });
  }

}
